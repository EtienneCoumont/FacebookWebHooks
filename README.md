# FacebookWebHooks

This project is a sample code to show how to use Facebook WebHooks with ASP.Net 5 Web API.

## Summary

Whenever something happen on your Facebook Page, Facebook will send a request to your server. (visit https://developers.facebook.com/docs/graph-api/webhooks to know more about Facebook Webhooks)

This Web Application catch this request and do something accordingly. For now, it just sends a mail with the changes.

## Prerequisites :

* Visual Studio 2015 (Community Version is freely available)
* An Azure Account (optional, you can publish your webapp wherever you want)
* A Facebook Application (you can create one for free here : https://developers.facebook.com/)
* You must be Admin of a page on Facebook

## Instructions

* Open the solution in Visual Studio
* Edit the configurations in appsettings.json
* 
### Local Tests



## Facebook Configuration :

### Create the Webhook

* Go to the Dashboard of your Facebook Application : https://developers.facebook.com/apps/
* Click the Webhooks menu
* New Subscription > Page
 * Callback Url : https://yourdomain.com/api/webhooks
 * Verify Token : The same one as in appsettings.json
 * Subscriptions Fields : Select the fields you're interested in
 
### Make your application subscribe to your page

* Open the Graph API Explorer : https://developers.facebook.com/tools/explorer/
* On the top right Combo Box, select your Application
* Just below, in the Token ComboBox, select your Page.
* Select the verb POST for the request
* Enter the path : {your-page-id}/subscribed_apps
* Submit : you should get a success.


## TODO :

* Signature verification
* More tests

 
