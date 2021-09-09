# Running .NET Core Apps in AWS App Mesh Lab/Workshop

## Warning
> Following these steps **will make changes in your current AWS Account**. Do it on your own risk. Authors of this guide provide this content as as, with **no guarantee whatsoever**, and **will not be liable** in any way if any damage is caused by your deciding to follow this guide.

## Overview

This document provides complete but simplified set of steps for running cross-platform .NET Core apps of this repo in AWS App Mesh on AWS Elastic Kubernetes Service (EKS). This guide should be compatible with Windows, MacOS and Linux.

Here's the high-level overview of the workshop plan.
- Provision Kubernetes (EKS) cluster.
- Deploy AWS App Mesh components on the EKS cluster.
- Create App Mesh aware app installers (Helm Charts) for sample apps in this repo, targeting EKS.
- Install color service and color client applications in the mesh-enabled EKS cluster.
- Run load test.
- Review performance and service dependency reports.  

## Before You Start

- Please review and install [required pre-requisites](https://github.com/aws-samples/eks-cdk-stack-dotnet/blob/main/src/eks-cdk-stack-dotnet/README.md).
- Commands below use cross-platform PowerShell.
- `aws` CLI is assumed to be configured to have administrator privileges on the chosen AWS Account and to point at a desired AWS Region.

# Steps

## Provision EKS Cluster

> Although it's customary to use `eksctl` CLI to provision EKS clusters, we will use our CDK stack to do so in order to generate and capture stack outputs required for subsequent installation of AWS App Mesh integration components.

- Assuming current directory is pointing to the root of this repository, save current path location by running:
```sh
Push-Location
``` 
- Create temporary directory for additional open-source assets required for this lab:
```sh
mkdir ~/demo-assets
cd ~/demo-assets
```
- Download .NET CDK stacks for provisioning EKS cluster and installing App Mesh integration components:
```sh
git clone https://github.com/aws-samples/eks-cdk-stack-dotnet.git
```
- cd into the directory with the CDK stack that provisions EKS cluster:
```sh
cd ./eks-cdk-stack-dotnet/src/eks-cdk-stack-dotnet/
```
- Ensure CDK can do its job by initializing its AWS staging resources. This needs to be done only once per account/region combination. 
```sh
cdk bootstrap
```
Ignore warnings, if any.

- Create AWS EKS Kubernetes cluster. 
  > If you are comfortable with both Kubernetes and AWS, feel free to [explore EKS CDK stack parameters](https://github.com/aws-samples/eks-cdk-stack-dotnet/blob/main/src/eks-cdk-stack-dotnet/cdk.json) and to modify the command below as you see fit.<br/>

  (Back-ticks are for PowerShell. For Linux shells use '\\'.)
```sh
$CLUSTER="rainbow-cluster"
$EKS_STACK="eks-create-rainbow-cluster"

cdk deploy `
    -c StackName=$EKS_STACK `
    -c EksClusterName=$CLUSTER `
    -c FargateNamespaces="" `
    -c OnDemandInstanceCount=1 `
    -c OnDemandInstanceType=t3.medium `
    -c SpotInstanceCount=3
```
When prompted "Do you wish to deploy these changes (y/n)?", proceed if you are comfortable with IAM and other changes the stack proposes.
> Provisioning of the EKS cluster will likely **take ~20 minutes**.

* After EKS cluster is provisioned, capture CDK stack outputs in the PowerShell variable:
```sh
$EKS_STACK_OUT=@{};(Get-CFNStack $EKS_STACK).Outputs | foreach { $EKS_STACK_OUT[$_.OutputKey]=$_.OutputValue }
```
* Authorize local `docker` CLI to push images to the Amazon Elastic Container Registry (ECR):
```sh
Invoke-Expression $EKS_STACK_OUT['DockerLoginForECR']
``` 
You should see "Login successful" output.

* Add kubeconfig entry to point local `kubectl` to the newly-created cluster:
```sh
$KUBECTL_CONFIG_CMD=$EKS_STACK_OUT[($EKS_STACK_OUT.Keys | Where { $_.StartsWith('eksclusterConfigCommand') })]
Invoke-Expression $KUBECTL_CONFIG_CMD
``` 
* Verify that your local `kubectl` CLI can communicate with the EKS cluster:
```sh
kubectl get pods --all-namespaces
```
You should see output looking like:
```txt
NAMESPACE     NAME                               READY   STATUS    RESTARTS   AGE
kube-system   aws-lb-controller-b7d56449-nltg8   1/1     Running   0          44m
kube-system   aws-lb-controller-b7d56449-sq46f   1/1     Running   0          44m
kube-system   aws-node-5t52c                     1/1     Running   0          47m
kube-system   aws-node-bl8rg                     1/1     Running   0          47m
kube-system   aws-node-pflcr                     1/1     Running   0          47m
kube-system   aws-node-zhwhp                     1/1     Running   0          19m
kube-system   coredns-7cfc675d7d-6sx4p           1/1     Running   0          53m
kube-system   coredns-7cfc675d7d-rvffw           1/1     Running   0          53m
kube-system   kube-proxy-6kgj7                   1/1     Running   0          47m
kube-system   kube-proxy-kmhdl                   1/1     Running   0          19m
kube-system   kube-proxy-mvp9h                   1/1     Running   0          47m
kube-system   kube-proxy-slzh9                   1/1     Running   0          47m
```

To work around a likely CDK bug, store `kubeclt` IAM role:
```sh
$KUBECTL_ROLE=$KUBECTL_CONFIG_CMD.Split('/')[-1]
```

## Deploy AWS App Mesh components on the EKS cluster

* cd into the directory with the AWS App Mesh CDK stack
```sh
cd ../cdk-app-mesh-eks-namespace
```

* Install AWS **App Mesh integration components** and **create the mesh**. (If you are interested in exploring App Mesh CDK stack parameters, they can be found [here](https://github.com/aws-samples/eks-cdk-stack-dotnet/blob/main/src/cdk-app-mesh-eks-namespace/cdk.json).)

```sh
$MESH_NAME="color-mesh"

cdk diff `
  -c StackName=install-color-app-mesh `
  `
  -c SkipAppMeshControllerInstallation=false `
  -c SkipCreatingAppMesh=false `
  -c AppMeshName=$MESH_NAME `
  -c MeshedNamespace="" `
  -c AddIngressGatewayToNs=false `
  `
  -c EksClusterName=$CLUSTER `
  -c ExistingVpcId=$($EKS_STACK_OUT['EksClusterVpcId']) `
  -c KubectlRole=$KUBECTL_ROLE `
  -c EksOidcProviderId=$($EKS_STACK_OUT['EksOidcConnectProviderARN'])
```

* Create **mesh-enabled K8s namespace** for our applications and add App Mesh Ingress Gateway, a.k.a Virtual Gateway, to enable usage of AWS ALB for ingressing external traffic into this namespace.

```sh
cdk diff `
  -c StackName=create-color-space-meshed-ns `
  `
  -c MeshedNamespace=color-space `
  -c SkipCreatingNamespace=false `
  -c MeshedNamespaceIsOnFargate=true `
  -c AppMeshName=$MESH_NAME `
  `
  -c EksClusterName=$CLUSTER `
  -c SkipLbControllerInstallation=true `
  -c ExistingVpcId=$($EKS_STACK_OUT['EksClusterVpcId']) `
  -c KubectlRole=$KUBECTL_ROLE `
  -c EksOidcProviderId=$($EKS_STACK_OUT['EksOidcConnectProviderARN'])
```