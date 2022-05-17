/*
List of Station x Instrument links with overlapping Date ranges
*/
use Observations;
with StationInstrumentLinks
as
(
Select 
  StationID, Station.Code StationCode, Station.Name StationName,
  InstrumentID, Instrument.Code InstrumentCode, Instrument.Name InstrumentName,
  Station_Instrument.StartDate, Station_Instrument.EndDate 
from
  Station_Instrument
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.Id)
)
Select distinct
  LeftLinks.InstrumentCode, /*LeftLinks.InstrumentName,*/ LeftLinks.StationCode, /*LeftLinks.StationName,*/ LeftLinks.StartDate, LeftLinks.EndDate,
  /*RightLinks.InstrumentCode, RightLinks.InstrumentName,*/ RightLinks.StationCode, /*RightLinks.StationName,*/ RightLinks.StartDate, RightLinks.EndDate
from
  StationInstrumentLinks LeftLinks
  inner join StationInstrumentLinks RightLinks
    on (LeftLinks.InstrumentID = RightLinks.InstrumentID) and (LeftLinks.StationID <> RightLinks.StationID) and
	   (LeftLinks.StartDate < RightLinks.EndDate) and (LeftLinks.EndDate > RightLinks.StartDate)
--where
--  (LeftLinks.InstrumentCode like '%10010801')
order by
  LeftLinks.InstrumentCode, LeftLinks.StationCode, RightLinks.StationCode, LeftLinks.StartDate, RightLinks.StartDate, LeftLinks.EndDate, RightLinks.EndDate