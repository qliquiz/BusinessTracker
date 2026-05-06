-- V005: Индексы на поле Name для Categories, Nomenclatures, Employees.
-- Ускоряют поиск новых сущностей при загрузке данных (EntityExtractor).

CREATE INDEX IF NOT EXISTS "IX_Categories_Name" ON "Categories" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Categories_OwnerId_Name" ON "Categories" ("OwnerId", "Name");

CREATE INDEX IF NOT EXISTS "IX_Nomenclatures_Name" ON "Nomenclatures" ("Name");

CREATE INDEX IF NOT EXISTS "IX_Employees_Name" ON "Employees" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Employees_OwnerId_Name" ON "Employees" ("OwnerId", "Name");
