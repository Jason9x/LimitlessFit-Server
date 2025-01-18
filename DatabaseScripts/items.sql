create table items
(
    id              int auto_increment
        primary key,
    image_url       varchar(255)   not null,
    name_key        varchar(100)   not null,
    description_key varchar(100)   not null,
    price           decimal(10, 2) not null
);

