CREATE TABLE IF NOT EXISTS "JournalRows"
(
    "Id"
    UUID
    NOT
    NULL
    DEFAULT
    gen_random_uuid
(
) PRIMARY KEY,
    "OrganizationId" UUID NOT NULL,
    "Code" BIGINT NOT NULL,
    "TypeCode" INT NOT NULL,
    "TransTypeName" TEXT NOT NULL DEFAULT '',
    "ReceiptNumber" INT NOT NULL DEFAULT 0,
    "ProductCode" BIGINT,
    "CategoryCode" BIGINT,
    "EmployeeCode" INT,
    "Period" TIMESTAMP NOT NULL,
    "Quantity" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "Price" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "Discount" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "RawId" INT NOT NULL DEFAULT 0,
    "RawLoginId" INT NOT NULL DEFAULT 0,
    "EmployeeName" TEXT NOT NULL DEFAULT '',
    "CategoryName" TEXT NOT NULL DEFAULT '',
    "NomenclatureName" TEXT NOT NULL DEFAULT ''
    );
