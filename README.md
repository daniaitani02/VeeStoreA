# VeeStore Part A

Before you begin, add the following SQL tables.

## Database Name: VeeStoreDb

### Table 1 (Products):
```
CREATE TABLE [dbo].[Product] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Price]       INT            NOT NULL,
    [Description] NVARCHAR (100) NULL,
    [Category]    NVARCHAR (30)  NULL,
    [ImageName]   NVARCHAR (50)  NOT NULL,
    [Status]      NVARCHAR (30)  NULL,
    [CreatedAt]   DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


```
### Table 2 (Customers): 
```
CREATE TABLE [dbo].[Customer] (
    [Email]       NVARCHAR (50) NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [Gender]      NVARCHAR (10) NULL,
    [PhoneNumber] NVARCHAR (20) NULL,
    [JoinedAt]    DATETIME      NOT NULL,
    [Status]      NVARCHAR (20) NULL,
    [CurrencyId]  INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Email] ASC),
    CONSTRAINT [FK_Table_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currency] ([Id])
);


```
### Table 3 (Carts):
```
CREATE TABLE [dbo].[Cart] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [CustomerEmail] NVARCHAR (50) NOT NULL,
    [Status]        NVARCHAR (50) NOT NULL,
    [PaidAt]        DATETIME      NOT NULL,
    [CreatedAt]     DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cart_Customer] FOREIGN KEY ([CustomerEmail]) REFERENCES [dbo].[Customer] ([Email])
);


```
### Table 4 (CartItems):
```
CREATE TABLE [dbo].[CartItem] (
 [Id] INT IDENTITY (1, 1) NOT NULL,
 [ProductId] INT NOT NULL,
 [CartId] INT NOT NULL,
 [Quantity] INT NOT NULL,
 [AddedAt] DATETIME NOT NULL,
 PRIMARY KEY CLUSTERED ([Id] ASC),
 CONSTRAINT [FK_Table_Cart] FOREIGN KEY ([CartId])
REFERENCES [dbo].[Cart] ([Id]),
 CONSTRAINT [FK_Table_Prodcut] FOREIGN KEY
([ProductId]) REFERENCES [dbo].[Product] ([Id])
);
GO
CREATE NONCLUSTERED INDEX
[IX_FK_CartItem_ProductId]
 ON [dbo].[CartItem]([ProductId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_FK_CartItem_CartId]
 ON [dbo].[CartItem]([CartId] ASC);
 ```
 ### Table 5 (CardCodes):
 ```
 CREATE TABLE [dbo].[CardCode] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [ProductId] INT            NOT NULL,
    [Code]      NVARCHAR (100) NOT NULL,
    [Status]    NVARCHAR (20)  NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [UsedAt]    DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Table_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_CardCode_ProductId]
    ON [dbo].[CardCode]([ProductId] ASC);


 ```
 
 ### Table 6 (CouponCodes):
 ```
 CREATE TABLE [dbo].[CouponCode] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [Code]               NVARCHAR (50) NOT NULL,
    [ExpiryDate]         DATETIME      NOT NULL,
    [CreatedAt]          DATETIME      NOT NULL,
    [DiscountPercentage] INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


 ```
 
 ### Table 7 (Currency)
 ```
 CREATE TABLE [dbo].[Currency] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [ShortName]  NVARCHAR (50) NULL,
    [Symbol]     NVARCHAR (10) NULL,
    [Flag]       NVARCHAR (20) NULL,
    [Multiplier] FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

 ```
 ### Table 8 (FAQs)
 ```
 CREATE TABLE [dbo].[Currency] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Question]  NVARCHAR (100) NOT NULL,
    [Answer]     NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
 ```
 ### Table 9 (Categories)
 ```
 CREATE TABLE [dbo].[Currency] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (30) NOT NULL,
    [Description]     NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
 ```
 ## Lastly, run this command in the package manager console:
 ```
 Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r
 ```

About VeeStore

VeeStore is an eCommerce platform that is specialized in selling digital content such as gift cards and online subscriptions, especially the highest quality and most needed from those products, and to make sure these products do match our standards, we only buy from the manufacturers themselves and check the authenticity of every product before making it available for sale.
Our desire is to create an online store fueled by people interest and passion, so we created a store where people can buy the latest and most requested gift cards easily and cost-friendly.


OUR VISION
Make daily life easier through technology.

OUR GOALS

1. Providing the most requested gift cards and subscriptions all in one place for our customers.
2. Selling these products at the best possible price.
3. Providing a unique and fun shopping experience for our customers and visitors
