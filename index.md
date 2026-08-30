---
layout: page
title: Home
description: This page should not be visible.
---
@using SitePlugin
Hello!

If you are seeing this page, my site was misconfigured or the site is being hosted
locally.

[Click here to see the latest comic.](@Model.Site.TryGetLastPost().Url)
