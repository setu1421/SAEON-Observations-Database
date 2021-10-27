Update InventorySnapshots set Downloads = 0 where Downloads is null
Alter table InventorySnapshots alter column Downloads int not null