---
layout: page
title: Credits
description: "Third party libraries that make this site possible."
comments: false
tags: [cake, credits, csharp, dotnet, foss, open, source, third, party]
---

@using Pretzel.Logic.Templating.Context

This website was created with the help of several third-party libraries and services.  They are listed below.

_None_ of these libraries or services are affiliated with @(Model.Site.Config["title"]).

## Binary Theme

CSS Theme

* Website: [https://binarytheme.com/](https://web.archive.org/web/20200310074347/https://binarytheme.com/)
* Theme: [https://binarytheme.com/bloggo-clean-personal-blog-html5-template-2/](https://web.archive.org/web/20221007152249/https://binarytheme.com/bloggo-clean-personal-blog-html5-template-2/)
* Terms of Service: [https://binarytheme.com/terms-of-service/](https://web.archive.org/web/20230203042420/https://binarytheme.com/terms-of-service/)

@Include( "credits.md", Model, typeof( PageContext ) )
