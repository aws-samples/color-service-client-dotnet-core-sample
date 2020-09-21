## Overview

This repository contains two simple ASP.NET Core sample applications - one UI and 
one REST service - in a single VS solution.

[REST service](./ServiceA/Controllers/ColorController.cs) returns configurable color name.

[Client app](./WebUiApp/Views/Home/Index.cshtml) calls the service at a configurable endpoint, when a link on a page is 
clicked.

These duo of samples is useful for demonstrating different containerization-
related use cases, like deploying applications to Kubernetes and to service
meshes, to developers with .NET background.

