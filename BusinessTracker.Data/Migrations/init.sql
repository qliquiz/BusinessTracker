-- Creating tables and indexes
CREATE TABLE IF NOT EXISTS "Organizations"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "Name" VARCHAR
(
    255
) NOT NULL,
    "Inn" VARCHAR
(
    10
) NOT NULL,
    "Address" TEXT NOT NULL,
    "LoadOptions" JSONB
    );

CREATE UNIQUE INDEX "OrganizationInnIx" ON "Organizations" ("Inn");

CREATE TABLE IF NOT EXISTS "Categories"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "Name" VARCHAR
(
    255
) NOT NULL,
    "OwnerId" UUID NOT NULL
    );

CREATE TABLE IF NOT EXISTS "Employees"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "Name" VARCHAR
(
    255
) NOT NULL,
    "PhoneNumber" VARCHAR
(
    20
),
    "OwnerId" UUID NOT NULL,
    "Role" INT NOT NULL DEFAULT 0
    );

CREATE TABLE IF NOT EXISTS "Nomenclatures"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "Name" VARCHAR
(
    255
) NOT NULL,
    "CategoryId" UUID NOT NULL
    );

CREATE TABLE IF NOT EXISTS "Transactions"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "Type" INT NOT NULL,
    "OwnerId" UUID NOT NULL,
    "TransactionDate" TIMESTAMP WITH TIME ZONE NOT NULL,
                                    "NomenclatureId" UUID NOT NULL,
                                    "EmployeeId" UUID NOT NULL,
                                    "Amount" NUMERIC (15, 2) NOT NULL,
    "Quantity" NUMERIC
(
    15,
    2
) NOT NULL,
    "Discount" NUMERIC
(
    15,
    2
) NOT NULL
    );

CREATE TABLE IF NOT EXISTS "Users"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "Name" VARCHAR
(
    255
) NOT NULL,
    "Password" TEXT NOT NULL
    );

CREATE TABLE IF NOT EXISTS "LinksUserOrganizations"
(
    "Id"
    UUID
    NOT
    NULL
    PRIMARY
    KEY
    DEFAULT
    gen_random_uuid
(
),
    "UserId" UUID NOT NULL,
    "OrganizationId" UUID NOT NULL
    );

-- Creating constraints

ALTER TABLE "LinksUserOrganizations"
    ADD CONSTRAINT "FK_LinksUserOrganizations_Users"
        FOREIGN KEY ("UserId")
            REFERENCES "Users" ("Id") ON DELETE CASCADE;

ALTER TABLE "LinksUserOrganizations"
    ADD CONSTRAINT "FK_LinksUserOrganizations_Organizations"
        FOREIGN KEY ("OrganizationId")
            REFERENCES "Organizations" ("Id") ON DELETE CASCADE;

ALTER TABLE "Categories"
    ADD CONSTRAINT "FK_Categories_Organizations"
        FOREIGN KEY ("OwnerId")
            REFERENCES "Organizations" ("Id") ON DELETE CASCADE;

ALTER TABLE "Employees"
    ADD CONSTRAINT "FK_Employees_Organizations"
        FOREIGN KEY ("OwnerId")
            REFERENCES "Organizations" ("Id") ON DELETE CASCADE;

ALTER TABLE "Nomenclatures"
    ADD CONSTRAINT "FK_Nomenclatures_Categories"
        FOREIGN KEY ("CategoryId")
            REFERENCES "Categories" ("Id") ON DELETE CASCADE;

ALTER TABLE "Transactions"
    ADD CONSTRAINT "FK_Transactions_Organizations"
        FOREIGN KEY ("OwnerId")
            REFERENCES "Organizations" ("Id") ON DELETE CASCADE;

ALTER TABLE "Transactions"
    ADD CONSTRAINT "FK_Transactions_Nomenclatures"
        FOREIGN KEY ("NomenclatureId")
            REFERENCES "Nomenclatures" ("Id") ON DELETE CASCADE;

ALTER TABLE "Transactions"
    ADD CONSTRAINT "FK_Transactions_Employees"
        FOREIGN KEY ("EmployeeId")
            REFERENCES "Employees" ("Id") ON DELETE CASCADE;
