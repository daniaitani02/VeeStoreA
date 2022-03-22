# VeeStore Part A

Before you begin, add the following SQL tables.

## Database Name: VeeStoreDb

### Table 1 (Products Table):
```
CREATE TABLE [dbo].[Product] (
 [Id] INT IDENTITY (1, 1) NOT NULL,
 [Name] NVARCHAR (50) NULL,
 [Price] INT NULL,
 PRIMARY KEY CLUSTERED ([Id] ASC)
);
```
### Table 2 (Customers Table): 
```
CREATE TABLE [dbo].[Customer] (
 [UserName] NVARCHAR (50) NOT NULL,
 [Name] NVARCHAR (50) NULL,
 [Address] NVARCHAR (50) NULL,
 PRIMARY KEY CLUSTERED ([UserName] ASC)
);
```
### Table 3 (Carts Table):
```
CREATE TABLE [dbo].[Cart] (
 [Id] INT IDENTITY (1, 1) NOT NULL,
 [CustomerName] NVARCHAR (50) NOT NULL,
 [Status] NVARCHAR (50) NOT NULL,
 PRIMARY KEY CLUSTERED ([Id] ASC),
 CONSTRAINT [FK_Cart_Customer] FOREIGN KEY
([CustomerName]) REFERENCES [dbo].[Customer]
([UserName])
);
```
### Table 4 (CartItems Table):
```
CREATE TABLE [dbo].[CartItem] (
 [Id] INT IDENTITY (1, 1) NOT NULL,
 [ProductId] INT NOT NULL,
 [CartId] INT NOT NULL,
 [Quantity] INT NOT NULL,
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
