-- Создаем таблицы и индексы
create table if not exists companies
(
    id      uuid not null primary key DEFAULT gen_random_uuid(),
    inn     text,
    address text
);

create unique index company_inn_ix on companies (inn);

create table if not exists categories
(
    id         uuid not null primary key DEFAULT gen_random_uuid(),
    name       text,
    company_id uuid
);

create table if not exists employees
(
    id         uuid not null primary key DEFAULT gen_random_uuid(),
    name       text,
    phone      text,
    company_id uuid
);

create table if not exists nomenclatures
(
    id          uuid not null primary key DEFAULT gen_random_uuid(),
    name        text,
    category_id uuid
);

create table if not exists transactions
(
    id               uuid                     not null primary key DEFAULT gen_random_uuid(),
    transaction_type int                      not null,
    company_id       uuid                     not null,
    change_period    timestamp with time zone NOT NULL,
    nomenclature_id  uuid,
    employee_id      uuid,
    price            numeric(15, 2),
    quantity         numeric(15, 2),
    discount         numeric(15, 2)
);

create table if not exists users
(
    id       uuid not null primary key DEFAULT gen_random_uuid(),
    name     text,
    password text
);

create table if not exists links_user_company
(
    id         uuid not null primary key DEFAULT gen_random_uuid(),
    user_id    uuid,
    company_id uuid
);

-- Создаем связи

alter table links_user_company
    add constraint links_user_company_user_id_fk
        foreign key (user_id)
            references users (id);

alter table links_user_company
    add constraint links_user_company_company_id_fk
        foreign key (company_id)
            references companies (id);

alter table categories
    add constraint categories_company_id_fk
        foreign key (company_id)
            references companies (id);


alter table employees
    add constraint employees_company_id_fk
        foreign key (company_id)
            references companies (id);

alter table nomenclatures
    add constraint nomenclatures_category_id_fk
        foreign key (category_id)
            references categories (id);

alter table transactions
    add constraint transactions_company_id_fk
        foreign key (company_id)
            references companies (id);

alter table transactions
    add constraint transactions_nomenclature_id_fk
        foreign key (nomenclature_id)
            references nomenclatures (id);

alter table transactions
    add constraint transactions_employee_id_fk
        foreign key (employee_id)
            references employees (id);