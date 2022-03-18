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
