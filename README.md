Documentum REST Services .Net Client Samples
===================

[![License: Apache 2](https://img.shields.io/badge/license-Apache%202.0-brightgreen.svg)](http://www.apache.org/licenses/LICENSE-2.0)

This .Net solution contains a reference implementation of Documentum REST Services clients and tests written in C# code. The purpose of this solution is to demonstrate one way to develop a hypermedia driven REST client to consume Documentum REST Services. It does NOT indicate that users could not develop a REST client using other technologies.

EMC shares the source code of this project for the technology sharing. If users plan to migrate the sample code to their products, they are responsible to maintain this part of the code in their products and should agree with license polices of the referenced libraries used by this sample project.

The solution contains four projects.

- **RestClient**
- **Tester**
- **AspNetWebFormsRestConsumer**
- **DroidMamarinTest**

The client samples have been verified against Documentum REST Services 7.2. For more information, please visit [Documentum REST space in EMC Community Network](https://community.emc.com/docs/DOC-32266).

### RestClient

This project implements a REST client as a DLL library. It implements all services in Documentum REST 7.2 . It uses the JSON media type. It supports HTTP Basic authentication and optionally supports Kerberos.

Here is the code to start a Documentum REST services from **Home Document**.
```C#
RestController client = new RestController(username, password);
HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
```

When you get a response from a resource, you get a `state` of that resource on the client side as well. Then you can perform further operations on that resource according to its available methods. For instance,
```C#
public Document ImportDocumentAsNewVersion(Document doc, Stream contentStream, String mimeType, GenericOptions checkinOptions)
{
    // If the document is not already checked out, check it out.
    if (!doc.IsCheckedOut())
    {
        doc = doc.Checkout();
    }
    Document checkinDoc = NewDocument(doc.GetPropertyString("object_name"));
    checkinOptions.SetQuery("format", doc.GetPropertyString("a_content_type"));
    checkinOptions.SetQuery("page", 0);
    checkinOptions.SetQuery("primary", true);
    return doc.CheckinMinor(checkinDoc, contentStream, mimeType, checkinOptions);
}
```


### Tester 

This project is a console application to show how end to end functions work (the source code here is a great example of how to handle use cases). It has been tested on Windows and Linux and should work on Mac as well.

<img src="Demo/dotnet-demo1.gif" width="800">


### AspNetWebFormsRestConsumer

This project is an asp.net web forms sample to consume the REST services. 

<img src="Demo/dotnet-demo2.gif" width="800">


### DroidXamarinTest 

This project uses [Xamarin](http://xamarin.com/) to show how the DocumentumRestClient dll can be used on mobile platforms. To use this project, you must use `Xamarin Studio` or have `Xamarin for Visual Studio` installed. It is definitely nothing fancy, just a basic concept of list cabinets, navigate folders, nothing more.


### QuickStart

__Step 1__

Open the `App.Config` file and find below XML section:
```xml
    <section name="restconfig" type="System.Configuration.NameValueSectionHandler,System"/>
```
If you are using `Visual Studio`, it should read this:
```xml
    <section name="restconfig" type="System.Configuration.NameValueSectionHandler"/>
```

Basically, one has a type with a `,System` in it and the other doesn't. It is a workaround for  [Mono](http://www.mono-project.com/) for now until they fix it, but having this `,System` in the config for Visual Studio causes an exception when loading the configuration file.
		
__Step 2__

Go to the `<restConfig>` section and become familiar with the descriptions and what the parameters do. To get started, all you need do here is set the `importFilesDirectory` value to a (Windows) `[driveletter]:\Path\To\Files directory or (Unix) /Path/To/Files`

The path you choose should have a number of files directly under this directory. The Tester will choose a number of files from here at random as samples to upload.

__Step 3__

You can set the `importEmailsDirectory` to the same value as `importFilesDirectory`, this is not currently used with base Rest services, it is only available with extensions enabled. If you need an extension for Rest that imports emails and splits out the attachment files, have your account rep contact michael.mccollough@emc.com.

__Step 4__

You can update the `defaultReSTHomeUri`, `defaultUsername`, `defaultRepositoryName`, `defaultPassword` values to the values for your environment. 

__Step 5__ 

If you set the `useDefault` parameter to true, it will not prompt you initially to enter the above information. If you set it to false, when you initially launch the Tester it will prompt you but allow you to hit enter to accept the default values you set in this configuration file.

> __Kerberos__
		If you Rest service is setup to use Kerberos, you can set the defaultUserName and defaultPassword to "" (blank) and the Tester will use your current Windows credentials to login to the repository.
		
To run the Tester program, edit the properties in the `App.config` file. Specifically, the location to get random files from. If you have a directory of hundreds of random files, the tester will choose the number of documents you specify (at runtime) at random from the directory.

You can ignore the random emails directory, this test will not be run unless you have the ReST extensions we have developed for importing email (splitting off attachments like WDK does). If you have TCS installed, the loader can also detect and report duplicate file imports and give you options to deal with them. In some cases, duplication is unavoidable; you need same file in different locations with different security. But having duplicate detection lets you decide what to do, as a programmer, with the duplicate data.

The Tester project is meant to be an example of how one might use the RestClient library. The `UseCaseTests.cs` class should be very good for mining use case code from for other projects.

## Cross-platform Compatability

We did a first round of work exposing the Model and Controller dlls (RestClient project) as COM (for use in Office VBA, Python, or any other COM aware language). The whole project has been tested under Windows and under Linux (using [Mono](http://www.mono-project.com/)).

The project compatability reports it is also compatible with .NetCore so it should work on Mobile as well. This
would include iOS, Droid, and Windows phones.


