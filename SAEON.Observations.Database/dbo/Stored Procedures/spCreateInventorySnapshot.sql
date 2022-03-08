CREATE PROCEDURE [dbo].[spCreateInventorySnapshot]
AS
BEGIN
	Insert into InventorySnapshots
	  (Organisations, Programmes, Projects, Sites, Stations, Instruments, Sensors, Phenomena, Offerings, UnitsOfMeasure, Variables, Datasets, Observations, Downloads)
	Select 
	  Organisations, Programmes, Projects, Sites, Stations, Instruments, Sensors, Phenomena, Offerings, UnitsOfMeasure, Variables, Datasets, Observations, Downloads
	from 
	  vInventorySnapshots
	Select top(1) * from InventorySnapshots order by [When] Desc
END