# Freshdesk.NET 

[![Travis](https://travis-ci.org/jjb3rd/Freshdesk.Net.svg)]()
[![NuGet](https://img.shields.io/nuget/v/Freshdesk.svg)]()
[![NuGet](https://img.shields.io/nuget/dt/Freshdesk.svg)]()

Freshdesk.Net is a .NET library for freshdesk.com's REST API.  It can be used to create contacts, customers and tickets (with attachments).  Freshdesk.Net also runs on Mono, so yay!

## Installation

Freshdesk.Net is available as a [NuGet Package](https://www.nuget.org/packages/Freshdesk/).  NuGet package manager the preferred method of integrating Freshdesk.Net into your project.

#### Visual Studio

Click on *Tools -> NuGet Package Manager -> Package Manager Console* and enter the following

	PM> Install-Package Freshdesk

#### Xamarin Studio

Click on *Project -> Add NuGet Packages* then search for "Freshdesk", check it off and click *Add Package*

## Usage

To use Freshdesk.Net you'll need an account on [Freshdesk.com](freshdesk.com).  When you create an account you'll get an instance url (typically: `http://YOURCOMPANY.freshdesk.com`).  Each user also has their own API Key.  Yours can be found by clicking on your profile image, then Profile Settings and it will be listed on the right hand side of the screen.  When you use Freshdesk.Net you'll need your site URL and your API Key.  It should be noted that all calls made with this library are made as if they're performed by the user whose API Key is used.

This library makes calls to the [Freshdesk REST API](http://freshdesk.com/api).  This documentation is helpful debugging your application or determining what values to send for various parameters.

All examples are written as if you've added the following using statement to your file...

```cs
using Freshdesk;
```

#### Setup

All functions in Freshdesk.Net are made through the instances of the FreshdeskService class. To create an instance of this class you'll need your API Key and your site url.

```cs
FreshdeskService freshdeskService = 
		new FreshdeskService(
			"YOUR_API_KEY", 
			new Uri("https://YOURCOMPANY.freshdesk.com"));
```

#### Tickets

http://freshdesk.com/api#ticket

##### Create Ticket

http://freshdesk.com/api#create_ticket

```cs
GetTicketResponse ticketResponse = freshdeskService.CreateTicket(new CreateTicketRequest()
{
    TicketInfo = new CreateTicketInfo()
    {
        Email = "wilecoyote@acme.com",
        Subject = "ACME Super Outfit won't fly!!!",
        Description = "I recently purchased an ACME Super Outfit because it was supposed to fly, but it's doesn't work!",
        Priority = 1,
        Status = 2
    }
});
```

##### Create Ticket with Attachment

http://freshdesk.com/api#ticket_attachment

```cs
var ticketResponse = freshdeskService.CreateTicketWithAttachment(new CreateTicketRequest
{
    TicketInfo = new CreateTicketInfo
    {
        Email = "roadrunner@acme.com",
        Subject = "Beep Beep",
        Description = "Beep Beep!",
        Priority = 1,
        Status = 2
    }
}, new Collection<Attachment>
{
    new Attachment
    {
        Content = File.OpenRead("beep.txt"),
        FileName = "beep.txt"
    },
    new Attachment
    {
        Content = File.OpenRead("beep.png"),
        FileName = "beep.png"
    }
});
```

#### Companies

http://freshdesk.com/api#companies_attributes

##### Create Customer

http://freshdesk.com/api#create_customer

```cs
GetCustomerResponse customerResponse = freshdeskService.CreateCustomer(new CreateCustomerRequest()
{
    Customer = new Customer()
    {
        Name = "ACME Corp.",
        Description = "The ACME Corporation"
    }
});
```

#### Users

http://freshdesk.com/api#companies_attributes

##### Create Customer

http://freshdesk.com/api#create_customer

```cs
GetUserResponse userResponse = freshdeskService.CreateUser(new CreateUserRequest()
{
    User = new User()
    {
        Name = "Bugs Bunny",
        Email = "bugs@acme.com"
    }
});
```
