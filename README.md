# LandscapeInstitute.Dynamics.IEntityGenerator
Generate IEntity Framework Friendly Models for your .NET Project

## Introduction

This is a quick project. Rather than using Latebound models. This was an easier altertive for a project. 

## Usage

- Enter your details such as login credentials for a CRM instance you wish to use. 
- Click "Download entities" to download a listing of current entities 
- Choose what entities and fields you want to use (some fields are required when selecting an entity)
- Finally click "generate C#" to generate optionset and entity models to your output directory. 

## Options

### Use Partial Classes
Apply models as partial classes or not?

### Entity / Optionset Directory Name
A sub folder you want to break down your entities / optionsets into within your output directory

### Output directory
Where to save your generated code **WARNING THIS WILL CLEAR THE ABOVE SUB-FOLDERS ON GENERATION** 

### Entity / Optionset Namespace
What namespace to place your generated code in

### URL / Username / Password
Used to connect to your dynamics URL

## Include Using Statements
Add these using statements to all generated code

## Warnings

Your details entered are all saved in a config.json file, in the same directory as the application. Passwords are NOT Encoded or hashed, they are plain text stored. 
I suggest having a dedicated account for this if used often with readonly access to entities but no other access. 

