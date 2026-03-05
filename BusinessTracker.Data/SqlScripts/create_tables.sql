-- Drop tables if they exist to ensure a clean start
DROP TABLE IF EXISTS "Transaction";
DROP TABLE IF EXISTS "User";
DROP TABLE IF EXISTS "Nomenclature";
DROP TABLE IF EXISTS "LoadingSettings";
DROP TABLE IF EXISTS "Employee";
DROP TABLE IF EXISTS "Category";
DROP TABLE IF EXISTS "Organization";

-- Create Organization table
CREATE TABLE IF NOT EXISTS "Organization"
(
    "Id"      UUID PRIMARY KEY,
    "Name"    VARCHAR(255) NOT NULL,
    "Inn"     VARCHAR(255) NOT NULL,
    "Address" VARCHAR(255) NOT NULL
);

-- Create Category table
CREATE TABLE IF NOT EXISTS "Category"
(
    "Id"      UUID PRIMARY KEY,
    "Name"    VARCHAR(255) NOT NULL,
    "OwnerId" UUID         NOT NULL,
    CONSTRAINT FK_Category_Organization FOREIGN KEY ("OwnerId") REFERENCES "Organization" ("Id")
);

-- Create Employee table
CREATE TABLE IF NOT EXISTS "Employee"
(
    "Id"          UUID PRIMARY KEY,
    "Name"        VARCHAR(255) NOT NULL,
    "PhoneNumber" VARCHAR(255),
    "OwnerId"     UUID         NOT NULL,
    "Role"        INTEGER      NOT NULL,
    CONSTRAINT FK_Employee_Organization FOREIGN KEY ("OwnerId") REFERENCES "Organization" ("Id")
);

-- Create LoadingSettings table
CREATE TABLE IF NOT EXISTS "LoadingSettings"
(
    "Id"            UUID PRIMARY KEY,
    "OwnerId"       UUID         NOT NULL,
    "Description"   VARCHAR(255) NOT NULL,
    "StartPosition" BIGINT       NOT NULL,
    "BatchSize"     BIGINT       NOT NULL,
    CONSTRAINT FK_LoadingSettings_Organization FOREIGN KEY ("OwnerId") REFERENCES "Organization" ("Id")
);

-- Create Nomenclature table
CREATE TABLE IF NOT EXISTS "Nomenclature"
(
    "Id"         UUID PRIMARY KEY,
    "Name"       VARCHAR(255) NOT NULL,
    "CategoryId" UUID         NOT NULL,
    CONSTRAINT FK_Nomenclature_Category FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id")
);

-- Create User table
CREATE TABLE IF NOT EXISTS "User"
(
    "Id"             UUID PRIMARY KEY,
    "Username"       VARCHAR(255) UNIQUE      NOT NULL,
    "PasswordHash"   VARCHAR(255)             NOT NULL,
    "Email"          VARCHAR(255) UNIQUE,
    "OrganizationId" UUID                     NOT NULL,
    "CreatedAt"      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt"      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT FK_User_Organization FOREIGN KEY ("OrganizationId") REFERENCES "Organization" ("Id")
);

-- Create Transaction table
CREATE TABLE IF NOT EXISTS "Transaction"
(
    "Id"              UUID PRIMARY KEY,
    "Type"            INTEGER                  NOT NULL,
    "Amount"          NUMERIC(18, 4)           NOT NULL,
    "Discount"        NUMERIC(18, 4)           NOT NULL,
    "Quantity"        NUMERIC(18, 4)           NOT NULL,
    "TransactionDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "OwnerId"         UUID                     NOT NULL,
    "NomenclatureId"  UUID                     NOT NULL,
    "EmployeeId"      UUID                     NOT NULL,
    CONSTRAINT FK_Transaction_Organization FOREIGN KEY ("OwnerId") REFERENCES "Organization" ("Id"),
    CONSTRAINT FK_Transaction_Nomenclature FOREIGN KEY ("NomenclatureId") REFERENCES "Nomenclature" ("Id"),
    CONSTRAINT FK_Transaction_Employee FOREIGN KEY ("EmployeeId") REFERENCES "Employee" ("Id")
);