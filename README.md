# FacebookWebHooks

This project is a sample code to show how to use Facebook WebHooks with ASP.Net Core Web API.


## Summary

Whenever something happen on your Facebook Page, Facebook will send a request to your server. (visit https://developers.facebook.com/docs/graph-api/webhooks to know more about Facebook Webhooks)

This Web Application catch this request and do something accordingly. For now, it just sends a mail with the changes.


## Prerequisites :

* Visual Studio 2017 (Community Version is downloadable for free)
* An Azure Account (get an evaluation account for free : https://account.windowsazure.com )
* A Facebook Application (you can create one for free : https://developers.facebook.com/ )
* You must be Admin of a page on Facebook


## Instructions

* Open the solution in Visual Studio
* Edit the configurations in appsettings.json
* Validate the configuration by Right clicking on wwwroot/test.html, show in browser, and do some basic tests
* Right-click on the project, "Publish..." and select your azure account. 


## Facebook Configuration :

### Create the Webhook

Note : This app has been tested with Facebook Webhooks v2.10

* Go to the Dashboard of your Facebook Application : https://developers.facebook.com/apps/
* Click the Webhooks menu
* New Subscription > Page
 * Callback Url : https://yourdomain.com/api/webhooks
 * Verify Token : The same one as in appsettings.json
 * Subscriptions Fields : Select the fields you're interested in
 
### Make your page subscribe to your app

* Open the Graph API Explorer : https://developers.facebook.com/tools/explorer/
* On the top right Combo Box, select your Application
* Just below, in the Token ComboBox, select your Page.
* Select the verb POST for the request
* Enter the path : {your-page-id}/subscribed_apps
* Submit : you should get a success.


## It's done !

From now on, every time a new status or photo is posted on your page, you will get an email.

Feel free to adapt this application to any scenario you can imagine. Like :
- When I post something on Facebook, create a new blog post on my Drupal / Wordpress website.
- When I add a photo to Facebook, put it on tumbler too.
- ...

## TODO :

* More tests for various potential events

 
