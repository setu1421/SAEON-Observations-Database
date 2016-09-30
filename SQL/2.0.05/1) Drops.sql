Alter table Instrument drop constraint UX_Instrument_StationID_Code;
Alter table Instrument drop constraint UX_Instrument_StationID_Name;
Alter table Instrument drop constraint FK_Instrument_Station;
Alter table Instrument drop column StationID;
Alter table Station drop constraint FK_Station_Site;
Drop index Station.IX_Station_SiteID;
Alter table Station drop column SiteID;
alter table Sensor drop constraint FK_Sensor_Instrument;
drop index IX_Sensor_InstrumentID on Sensor
alter table Sensor drop column InstrumentID;
drop table Programme_Project;
drop table Site_Project;
drop table Site_Station;
drop table Site_Organisation;
drop table Station_Organisation;
drop table Instrument_Organisation;