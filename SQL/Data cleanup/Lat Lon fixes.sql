--Select * from Station where Latitude >= 0
--Select * from Station where Longitude <= 0
Update Station set Latitude = -Latitude where Latitude > 0
Update Station set Longitude = -Longitude where Longitude < 0
Update Observation set Latitude = -Latitude where Latitude > 0
Update Observation set Longitude = -Longitude where Longitude < 0
Update ImportBatchSummary set LatitudeNorth = -LatitudeNorth where LatitudeNorth > 0
Update ImportBatchSummary set LatitudeSouth = -LatitudeSouth where LatitudeSouth > 0
Update ImportBatchSummary set LongitudeWest = -LongitudeWest where LongitudeWest < 0
Update ImportBatchSummary set LongitudeEast = -LongitudeEast where LongitudeEast < 0

Update Instrument Set Latitude = -33.984972 where Name = 'SACTN Oudekraal DAFF UTR'