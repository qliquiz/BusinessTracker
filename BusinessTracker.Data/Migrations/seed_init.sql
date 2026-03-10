-- Seed data for Organization
INSERT INTO "companies" ("id", "inn", "address")
VALUES ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', '1234567890',
        '190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12'),
       ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12', '0987654321',
        '230000, Иркутская обл., Октябрьский р-н, г. Иркутск, ул. 3 Июля, д. 3')
ON CONFLICT ("id") DO NOTHING;
