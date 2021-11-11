Running .NET Core Apps in AWS App Mesh Lab/Workshop
===================================================

Warning
-------

> Following these steps **will make changes in your current AWS
> Account**. Do it on your own risk. Authors of this guide provide this
> content as as, with **no guarantee whatsoever**, and **will not be
> liable** in any way if any damage is caused by your deciding to follow
> this guide.

Overview
--------

This document provides complete but simplified set of steps for running
these cross-platform .NET Core in AWS App Mesh on AWS
Elastic Kubernetes Service (EKS). This guide should be compatible with
Windows, MacOS and Linux.

Here's the high-level overview of the workshop plan. 
* Provision Kubernetes (EKS) cluster. 
* Deploy AWS App Mesh components on the EKS cluster. 
* Create App Mesh aware app installers (Helm Charts) for sample
apps in this repo, targeting EKS.
* Install color service and color
client applications in the mesh-enabled EKS cluster.
* Run load test. 
* Review performance and service dependency reports.

Before You Start
----------------

-   Please review and install [required
    pre-requisites](https://github.com/aws-samples/eks-cdk-stack-dotnet/blob/main/src/eks-cdk-stack-dotnet/README.md).
-   Commands below use cross-platform PowerShell.
-   `aws` CLI is assumed to be configured to have administrator
    privileges on the chosen AWS Account and to point at a desired AWS Account and Region.

Steps
=====

* Enable AWS PowerShell extensions:
```sh
import-module AWSPowerShell.netcore
```

Provision EKS Cluster
---------------------


-   Create AWS EKS Kubernetes cluster.

```sh
$CLUSTER="app-mesh-demo"
eksctl create cluster --name $CLUSTER --appmesh-access --alb-ingress-access
```

> Provisioning of the EKS cluster will likely **take \~20 minutes**.

After EKS cluster provisioning has completed, verify that your local `kubectl` CLI can communicate with the EKS
    cluster:

``` sh
kubectl get pods --all-namespaces
```

You should see output looking like:

``` txt
NAMESPACE     NAME                      READY   STATUS    RESTARTS   AGE
kube-system   aws-node-tfwxl            1/1     Running   0          18m
kube-system   aws-node-xv6j7            1/1     Running   0          18m
kube-system   coredns-85cc4f6d5-vglv9   1/1     Running   0          27m
kube-system   coredns-85cc4f6d5-ws5n7   1/1     Running   0          27m
kube-system   kube-proxy-kxdsm          1/1     Running   0          18m
```

> Also, please consider using Kubernetes extension for Visual Studio Code to monitor and manage your K8s cluster. This way you could point & click instead of typing lots of `kubectl` commands.

* Associate an OpenID provider with the cluster:
```sh
eksctl utils associate-iam-oidc-provider --cluster $CLUSTER --approve
```

* Store cluster information in the PowerShell variable named `ClusterInfo`:
```sh
eksctl get cluster $CLUSTER -o=json | ConvertFrom-Json | Set-Variable ClusterInfo
```

* Use that cluster information from the previous step to store a few bits of data needed for adding AWS App Mesh components to the cluster.

``` sh
$KubectlRole=$ClusterInfo.RoleArn.Split('/')[-1]
$OidcId=$ClusterInfo.Identity.Oidc.Issuer.Split('/')[-1]
$ClusterVpc=$ClusterInfo.ResourcesVpcConfig.VpcId
```

Deploy AWS App Mesh components on the EKS cluster
-------------------------------------------------

``` sh
Push-Location
```

-   Create temporary directory for additional open-source assets
    required for this lab:

``` sh
mkdir ~/demo-assets
cd ~/demo-assets
```

-   Download .NET CDK stacks for provisioning EKS cluster and installing
    App Mesh integration components:

``` sh
git clone https://github.com/aws-samples/eks-cdk-stack-dotnet.git
```

* cd into the directory with the CDK stack:
```sh
cd ./eks-cdk-stack-dotnet/src
```

-   Ensure CDK can do its job by initializing its AWS staging resources.
    This needs to be done only once per account/region combination.

``` sh
cdk bootstrap
```

Ignore warnings, if any.

-   Install AWS **App Mesh integration components** and **create the
    mesh**. (If you are interested in exploring App Mesh CDK stack
    parameters, they can be found
    [here](https://github.com/aws-saamples/eks-cdk-stack-dotnet/blob/main/src/cdk-app-mesh-eks-namespace/cdk.json).)

``` sh
$MESH_NAME="color-mesh"

cdk diff `
  -c StackName=install-color-app-mesh `
  `
  -c SkipAppMeshControllerInstallation=false `
  -c SkipCreatingAppMesh=false `
  -c SkipLbControllerInstallation=false `
  -c AppMeshName=$MESH_NAME `
  -c MeshedNamespace="" `
  `
  -c EksClusterName=$CLUSTER `
  -c ExistingVpcId=$ClusterVpc `
  -c KubectlRole=$KubectlRole `
  -c EksOidcProviderId=$OidcId
```

-   Create **mesh-enabled K8s namespace** for our applications and add
    App Mesh Ingress Gateway, a.k.a Virtual Gateway, to enable usage of
    AWS ALB for ingressing external traffic into this namespace.

``` sh
cdk diff `
  -c StackName=create-color-space-meshed-ns `
  `
  -c AppMeshName=$MESH_NAME `
  -c MeshedNamespace=color-space `
  -c SkipCreatingNamespace=false `
  `
  -c EksClusterName=$CLUSTER `
  -c ExistingVpcId=$ClusterVpc `
  -c KubectlRole=$KubectlRole `
  -c EksOidcProviderId=$OidcId
```
