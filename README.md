# FacebookWebHooks

This project shows how to use Facebook WebHooks with ASP.Net 5 Web API

## Prerequisites :

* Visual Studio 2015 (Community Version is freely available)
* An Azure Account (optional, you can publish your webapp wherever you want)
* A Facebook Application (you can create one for free here : https://developers.facebook.com/)

## Configuration :

* Go to the Dashboard of your Facebook Application : https://developers.facebook.com/apps/
* Click the Webhooks menu
* New Subscription > Page
 * Callback Url : https://yourdomain.com/api/webhooks
 * Verify Token : The same one as in appsettings.json
 * Subscriptions Fields : Select the fields you're interested in
 
 
 
