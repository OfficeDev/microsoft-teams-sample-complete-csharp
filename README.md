# Steps to run locally

## Prerequisites

* Install Git for windows: https://git-for-windows.github.io/

* Clone this repo:<br>
    ```
    git clone https://github.com/OfficeDev/microsoft-teams-template-bot-CSharp.git
    ```

* Install Visual Studio and launch it as an administrator

* Build the solution to download all configured NuGet packages

* (Only needed if wanting to run in Microsoft Teams)<br>
Install some sort of tunnelling service. These instructions assume you are using ngrok: https://ngrok.com/

* (Only needed if wanting to run in the Bot Emulator)<br>
Install the Bot Emulator - click on "Bot Framework Emulator (Mac and Windows)": https://docs.botframework.com/en-us/downloads/#navtitle  
    * NOTE: make sure to pin the emulator to your task bar because it can sometimes be difficult to find again 

## Steps to see the bot running in the Bot Emulator<br>
NOTE: Teams does not work nor render things exactly like the Bot Emulator, but it is a quick way to see if your bot is running and functioning correctly.

1. Open the template-bot-master-csharp.sln solution with Visual Studio

2. In Visual Studio click the “Start Debugging” button (should be defaulted to running the Microsoft Edge configuration) 

3. Once the code is running, connect with the Bot Emulator to the default endpoint, "http://localhost:3979/api/messages", leaving "Microsoft App ID" and "Microsoft App Password" blank

Congratulations!!! You can now chat with the bot in the Bot Emulator!

## Steps to see the full app in Microsoft Teams

1. Begin your tunnelling service to get an https endpoint. 

	* Open a new **Command Prompt** window. 

	* Change to the directory that contains the ngrok.exe application. 

	* Run the command `ngrok http [port] --host-header=localhost` (you'll need the https endpoint for the bot registration) e.g.<br>
		```
		ngrok http 3979 --host-header=localhost
		```

	* The ngrok application will fill the entire prompt window. Make note of the Forwarding address using https. This address is required in the next step. 

	* Minimize the Command Prompt window that’s running ngrok. It’s no longer referenced in this article, but it must remain running.

2. Register a bot in the Microsoft Bot Framework.

Bots in Teams must be built upon the Microsoft Bot Framework. For this sample, as part of the package download process, you’ll get the   Bot Framework SDK and the Microsoft Teams extensions to Bot Framework.
  
In addition, every bot must be registered in the Bot Framework, so it is accessible by the services it uses like Microsoft Teams. Our  samples are designed for you to run yourself, so you’ll need to create your own bot, which also includes a Microsoft App ID and  password. Here’s how: 

 * Using your work or school account, sign in to the Microsoft Bot Framework site https://dev.botframework.com/bots/new.
 
 * Display name – Give your app a name. This does not have to be unique. This will be the name displayed in Teams. We recommend that you make this the same name as your app name in the manifest (this sample uses Sample-App-csharp). 
	> **NOTE**: If you decide to change the Display Name or icon after your bot is registered, it may take some time before your new name or icon will show up in your Teams client; logging out and logging back in will usually accelerate this.
	
 * Bot handle – Create a unique identifier for your bot.
	> **NOTE**: This cannot be changed and is not visible to users. If you change the Display name of your bot, your Bot handle will remain the same.
	
 * Long description – Enter a long description which may appear in channels or directories.
	> **NOTE**: In Microsoft Teams, the Store information will come from the Seller Dashboard.

 Next, you need to configure your bot’s service endpoint so Microsoft Teams knows how to connect to your bot:
 
 * Messaging endpoint – Paste the ngrok URL that you copied to the clipboard and append the appropriate endpoint to it. For our samples, again, our code is listening for messages on "/api/messages”, so for example you’d enter “https://2d1224fb.ngrok.io/api/messages”
 
 * Create Microsoft App ID and password – This button will take you to the Application Registration Portal, where you will create a unique Microsoft App ID and password.
    * App name – This will be filled in from what you entered in the previous step
    
    * App ID – This is a unique GUID created for your app, e.g. 93fed3d5-6782-462e-8a58-6a3e83ca6eab
    
    * Generate an app password to continue – Click this button to generate an app password (you’ll sometimes see this called an app secret), e.g. qgSctpqT89ZdfAymt66Ukgf
    
	> **NOTE**: You’ll need to copy and save this in a secure location as you will need this, and the App ID later. The app password will only be shown once.
	
    * Click the “Finish and go back to Bot Framework” button. 
    
    * You’ll return to the Registration page, with the App ID filled in, that matches the one created above. Check the box at the bottom to agree to the terms of use, and click “Register” to create your new accessible Bot Framework bot.
    
    * Click on the Microsoft Teams icon under “Add a featured channel.” 
    
    * Check the box to agree to the Terms of Service. 
    
    * Click “Done” on the Configure MS Teams page in the bottom left hand corner.
    
 * Bots and Microsoft Azure – When you edit the properties of an existing bot in the list of your bots in Bot Framework such as its messaging endpoint, which is common when first developing a bot, especially if you use ngrok, you will see the "Migration status" column and a blue "Migrate" button that will take you into the Microsoft Azure portal. Don't click on the "Migrate" button unless that's what you want to do; instead, click on the name of the bot and you can edit its properties. 
 
    * If you register your bot using Microsoft Azure, it does not need to be hosted on Microsoft Azure.
    
    * If you do register a bot using Microsoft Azure portal, you must have a Microsoft Azure account. You can create one for free. To verify your identity when you create one, you must provide a credit card, but it won't be charged; it's always free to create and use bots with Microsoft Teams.

3. You project needs to run with a configuration that matches your registered bot's configuration. To do this, you will need to update the web.config file:

	* In Visual Studio, open the Web.config file. Locate the `<appSettings>` section. 
 
	* Enter the BotId value. The BotId is the **Bot handle** from the **Configuration** section of the bot registration. 
 
	* Enter the MicrosoftAppId. The MicrosoftAppId is the app ID from the **Configuration** section of the bot registration. 
 
	* Enter the MicrosoftAppPassword. The MicrosoftAppPassword is the auto-generated app password displayed in the pop-up during bot registration.
	
	* Enter the BaseUri. The BaseUri is the https endpoint generated from ngrok.

	Here is an example for reference:
	
		<add key="BotId" value="Bot_Handle_Here" />
		<add key="MicrosoftAppId" value="88888888-8888-8888-8888-888888888888" />
		<add key="MicrosoftAppPassword" value="aaaa22229999dddd0000999" />
		<add key="BaseUri" value="https://#####abc.ngrok.io" />

4. In Visual Studio click the play button (should be defaulted to running the Microsoft Edge configuration)

5. Once the app is running, a manifest file is needed:
    * On the solution explorer of Visual Studio, navigate to the file, manifest/manifest.json - change:
        * <<REGISTERED_BOT_ID>> (there are 3) change to your registered bot's app ID
        * <<BASE_URI>> (there are 2) change to your https endpoint from ngrok
        * <<BASE_URI_DOMAIN>> (there is 1) change to your https endpoint from ngrok excluding the "https://" part
		
    * Save the file and zip this file and the bot_blue.png file (located next to it) together to create a manifest.zip file

6. Once complete, sideload your zipped manifest to a team as described here (open in a new browser tab): https://msdn.microsoft.com/en-us/microsoft-teams/sideload

Congratulations!!! You have just created and sideloaded your first Microsoft Teams app! Try adding a configurable tab, at-mentioning your bot by its registered name, or viewing your static tabs.<br><br>
NOTE: Most of this sample app's functionality will now work. The only limitations are the authentication examples because your app is not registered with AAD nor Visual Studio Team Services.

# Overview

This project is meant to help a Teams developer in two ways.  First, it is meant to show many examples of how an app can integrate into Teams.  Second, it is meant to give a set of patterns, templates, and tools that can be used as a starting point for creating a larger, scalable, more enterprise level bot to work within Teams.  Although this project focuses on creating a robust bot, it does include simples examples of tabs as well as examples of how a bot can give links into these tabs.

# What it is

At a high level, this project is written in C#, built to run a .Net, and uses the BotFramework to handle the bot's requests and responses. This project is designed to be run in Visual Studio using its debugger in order to leverage breakpoints. Most directories will hold a README file which will describe what the files within that directory do.
The easiest way to get started is to follow the steps listed in the "Steps to get started running the Bot Emulator". Once this is complete and running, the easiest way to add your own content is to create a new dialog in src/dialogs by copying one from src/dialogs/examples, change it accordingly, and then instantiate it with the others in the RootDialog.cs.

# General Architecture

Most code files that need to be compile reside in the src directory. Most files outside of the src directory are static files used for either configuration or for providing static resources to tabs, e.g. images and html.

# Files and Directories

* **manifest**<br><br>
This directory holds the skeleton of a manifest.json file that can be altered in order sideload this application into a team.

* **middleware**<br><br>
This directory holds the stripping at mention for channel class and Invoke message processing.

* **public**<br><br>
This directory holds static html, image, and javascript files used by the tabs and bot.  This is not the only public directory that is used for the tabs, though.  This directory holds the html and javascript used for the configuration page of the configurable tab.  The main content of the static and configurable comes from the static files placed in /public/tab/tabConfig.

* **src**<br><br>
This directory holds all the code files, which run the entire application.

* **utility**<br><br>
This directory holds utility functions for the project.

* **web.config**<br><br>
This file is a configuration file that can be used to update the config keys globally used in Application.

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
