Declare @TimUserId UniqueIdentifier = (Select UserId from aspnet_Users where LoweredUserName = 'tim')
Insert into Status (Code, Name, Description, UserID)
Values
  ('QA-97','Unverified - Historical','Historical data not yet verfied',@TimUserId),
  ('QA-98','Unverified - Staging','Staging data merged as unverfied, not in data log',@TimUserId)
Insert into StatusReason (Code, Name, Description, UserID)
Values
('QAR-01','Equipment theft','Equipment theft',@TimUserId),
('QAR-02','Rain gauge overflowed','Rain gauge overflowed',@TimUserId),
('QAR-03','Rain gauge leaking','Rain gauge leaking',@TimUserId),
('QAR-04','Rain gauge blocked','Rain gauge blocked',@TimUserId),
('QAR-05','Rain gauge accidentally tipped','Rain gauge accidentally tipped',@TimUserId),
('QAR-06','Casella clock in for repairs','Casella clock in for repairs',@TimUserId),
('QAR-07','Casella clock stopped','Casella clock stopped',@TimUserId),
('QAR-08','Clock running slow/fast','Clock running slow/fast',@TimUserId),
('QAR-09','Chart/Pen technical problems','Chart/Pen technical problems',@TimUserId),
('QAR-10','Animal/human disturbance','Animal/human disturbance',@TimUserId),
('QAR-11','Rain gauge did not siphon','Rain gauge did not siphon',@TimUserId),
('QAR-12','Chart time inaccuracy','Chart time inaccuracy',@TimUserId),
('QAR-13','Pen placed at wrong height','Pen placed at wrong height',@TimUserId),
('QAR-14','Calibration error','Calibration error',@TimUserId),
('QAR-15','Calibration required','Calibration required',@TimUserId),
('QAR-16','Hygrograph hair wet','Hygrograph hair wet',@TimUserId),
('QAR-17','Pen affected by high wind','Pen affected by high wind',@TimUserId),
('QAR-18','Chart not changed timeously','Chart not changed timeously',@TimUserId),
('QAR-19','Data lost due to logger swap','Data lost due to logger swap',@TimUserId),
('QAR-20','Power supply problem','Power supply problem',@TimUserId),
('QAR-21','Data logger technical problems','Data logger technical problems',@TimUserId),
('QAR-22','Stilling well silted up','Stilling well silted up',@TimUserId),
('QAR-23','Instrument installation incorrect','Instrument installation incorrect',@TimUserId),
('QAR-24','Faulty sensor','Faulty sensor',@TimUserId),
('QAR-25','Weir cleaning','Weir cleaning',@TimUserId),
('QAR-26','Memory problems/overwrite','Memory problems/overwrite',@TimUserId),
('QAR-27','No stream flow','No stream flow',@TimUserId),
('QAR-28','Weir silted up','Weir silted up',@TimUserId),
('QAR-29','Human data recording error','Human data recording error',@TimUserId),
('QAR-30','Incorrect configuration','Incorrect configuration',@TimUserId),
('QAR-31','Recording discontinued','Recording discontinued',@TimUserId),
('QAR-32','Fire damage','Fire damage',@TimUserId),
('QAR-33','Obstruction in V-notch','Obstruction in V-notch',@TimUserId),
('QAR-34','Dirty sensor','Dirty sensor',@TimUserId),
('QAR-35','Offset entered','Offset entered',@TimUserId),
('QAR-36','Offset not entered','Offset not entered',@TimUserId),
('QAR-37','Error with unknown cause','Error with unknown cause',@TimUserId),
('QAR-38','Historical data not yet verfied','Historical data not yet verfied',@TimUserId),
('QAR-39','Staging data merged as unverified','Staging data merged as unverified',@TimUserId)
