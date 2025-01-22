CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `migration_id` varchar(150) NOT NULL,
    `product_version` varchar(32) NOT NULL,
    PRIMARY KEY (`migration_id`)
);

START TRANSACTION;
CREATE TABLE `items` (
    `id` int NOT NULL AUTO_INCREMENT,
    `image_url` varchar(512) NOT NULL,
    `name_key` varchar(100) NOT NULL,
    `description_key` varchar(100) NOT NULL,
    `price` decimal(10,2) NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `orders` (
    `id` int NOT NULL AUTO_INCREMENT,
    `customer_name` varchar(100) NOT NULL,
    `order_date` datetime(6) NOT NULL,
    `total_price` decimal(10,2) NOT NULL,
    `status` int NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `users` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(100) NOT NULL,
    `email` varchar(255) NOT NULL,
    `password` varchar(128) NOT NULL,
    `failed_login_attempts` int NOT NULL,
    `lockout_end` datetime(6) NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `order_items` (
    `id` int NOT NULL AUTO_INCREMENT,
    `order_id` int NOT NULL,
    `item_id` int NOT NULL,
    `quantity` int NOT NULL,
    PRIMARY KEY (`id`),
    CONSTRAINT `fk_order_items_items_item_id` FOREIGN KEY (`item_id`) REFERENCES `items` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_order_items_orders_order_id` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`) ON DELETE CASCADE
);

CREATE INDEX `ix_order_items_item_id` ON `order_items` (`item_id`);

CREATE INDEX `ix_order_items_order_id` ON `order_items` (`order_id`);

CREATE UNIQUE INDEX `ix_users_email` ON `users` (`email`);

INSERT INTO `__EFMigrationsHistory` (`migration_id`, `product_version`)
VALUES ('20250122032216_InitialCreate', '9.0.1');

COMMIT;

