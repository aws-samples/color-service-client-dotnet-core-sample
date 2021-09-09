## Overview

This repository contains two simple ASP.NET Core sample applications - one UI and 
one REST service - in a single VS solution.

[REST service](./ServiceA/Controllers/ColorController.cs) returns configurable color name.

[Client app](./WebUIApp/Views/Home/Index.cshtml) calls the service at a configurable endpoint, when a link on a page is 
clicked.

These duo of samples is useful for demonstrating different containerization-
related use cases, like deploying applications to Kubernetes and to service
meshes, to developers with .NET background.

## Running these apps in AWS App Mesh on Amazon EKS cluster

[This guide](./docs/README.MD) takes you through steps of provisioning EKS cluster, installing AWS App Mesh components on it, creating Helm Chart installer for the sample apps in these repo, targeting App Mesh enabled Kubernetes clusters, and finally installing & testing the apps. 

## Security

See [CONTRIBUTING](CONTRIBUTING.md#security-issue-notifications) for more information.

## License

This library is licensed under the MIT-0 License. See the [LICENSE](./LICENSE) file.
