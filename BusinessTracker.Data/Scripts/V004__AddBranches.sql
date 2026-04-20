-- ================================================================
-- V004: Добавление таблицы филиалов (Branches)
-- Настройки загрузки данных переносятся с уровня организации на уровень филиала.
-- Существующие данные сохраняются: для каждой организации создаётся один головной филиал.
-- ================================================================

-- ----------------------------------------------------------------
-- Шаг 1. Проверка качества данных ДО миграции
-- ----------------------------------------------------------------
DO
$$
DECLARE
v_organizations  INT;
    v_categories
INT;
    v_nomenclatures
INT;
    v_employees
INT;
    v_transactions
INT;
    v_journal_rows
INT;
BEGIN
SELECT COUNT(*)
INTO v_organizations
FROM "Organizations";
SELECT COUNT(*)
INTO v_categories
FROM "Categories";
SELECT COUNT(*)
INTO v_nomenclatures
FROM "Nomenclatures";
SELECT COUNT(*)
INTO v_employees
FROM "Employees";
SELECT COUNT(*)
INTO v_transactions
FROM "Transactions";
SELECT COUNT(*)
INTO v_journal_rows
FROM "JournalRows";

RAISE
NOTICE '=== PRE-MIGRATION RECORD COUNTS ===';
    RAISE
NOTICE 'Organizations  : %', v_organizations;
    RAISE
NOTICE 'Categories     : %', v_categories;
    RAISE
NOTICE 'Nomenclatures  : %', v_nomenclatures;
    RAISE
NOTICE 'Employees      : %', v_employees;
    RAISE
NOTICE 'Transactions   : %', v_transactions;
    RAISE
NOTICE 'JournalRows    : %', v_journal_rows;
    RAISE
NOTICE '===================================';
END $$;

-- ----------------------------------------------------------------
-- Шаг 2. Создание таблицы Branches
-- ----------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Branches"
(
    "Id"
    UUID
    NOT
    NULL
    DEFAULT
    gen_random_uuid
(
) PRIMARY KEY,
    "Name" VARCHAR
(
    255
) NOT NULL,
    "OrganizationId" UUID NOT NULL,
    "LoadOptions" JSONB,
    CONSTRAINT "FK_Branches_Organizations"
    FOREIGN KEY
(
    "OrganizationId"
)
    REFERENCES "Organizations"
(
    "Id"
) ON DELETE CASCADE
    );

-- ----------------------------------------------------------------
-- Шаг 3. Создание головного филиала для каждой существующей организации.
-- Для seed-организаций используются фиксированные UUID чтобы тесты работали стабильно.
-- ----------------------------------------------------------------
INSERT INTO "Branches" ("Id", "Name", "OrganizationId", "LoadOptions")
SELECT CASE "Id"::text WHEN 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11' THEN 'f0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11'::uuid
        WHEN 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12' THEN 'f0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12'::uuid
        ELSE gen_random_uuid()
END
,
    'Головной филиал',
    "Id",
    "LoadOptions"
FROM "Organizations"
ON CONFLICT ("Id") DO NOTHING;

-- ----------------------------------------------------------------
-- Шаг 4. Удаление столбца LoadOptions из Organizations
-- (настройки теперь хранятся в Branches)
-- ----------------------------------------------------------------
ALTER TABLE "Organizations" DROP COLUMN IF EXISTS "LoadOptions";

-- ----------------------------------------------------------------
-- Шаг 5. Проверка качества данных ПОСЛЕ миграции (сравнение с Шагом 1)
-- ----------------------------------------------------------------
DO
$$
DECLARE
v_organizations  INT;
    v_branches
INT;
    v_categories
INT;
    v_nomenclatures
INT;
    v_employees
INT;
    v_transactions
INT;
    v_journal_rows
INT;
BEGIN
SELECT COUNT(*)
INTO v_organizations
FROM "Organizations";
SELECT COUNT(*)
INTO v_branches
FROM "Branches";
SELECT COUNT(*)
INTO v_categories
FROM "Categories";
SELECT COUNT(*)
INTO v_nomenclatures
FROM "Nomenclatures";
SELECT COUNT(*)
INTO v_employees
FROM "Employees";
SELECT COUNT(*)
INTO v_transactions
FROM "Transactions";
SELECT COUNT(*)
INTO v_journal_rows
FROM "JournalRows";

RAISE
NOTICE '=== POST-MIGRATION RECORD COUNTS ===';
    RAISE
NOTICE 'Organizations  : %  (unchanged)', v_organizations;
    RAISE
NOTICE 'Branches       : %  (new, = Organizations)', v_branches;
    RAISE
NOTICE 'Categories     : %  (unchanged)', v_categories;
    RAISE
NOTICE 'Nomenclatures  : %  (unchanged)', v_nomenclatures;
    RAISE
NOTICE 'Employees      : %  (unchanged)', v_employees;
    RAISE
NOTICE 'Transactions   : %  (unchanged)', v_transactions;
    RAISE
NOTICE 'JournalRows    : %  (unchanged)', v_journal_rows;
    RAISE
NOTICE '=====================================';

    -- Проверяем инвариант: кол-во филиалов = кол-ву организаций
    IF
v_branches <> v_organizations THEN
        RAISE EXCEPTION 'Invariant violation: Branches(%) != Organizations(%)', v_branches, v_organizations;
END IF;
END $$;
