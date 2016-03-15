
CREATE VIEW [dbo].[vDataQuery]
AS
SELECT     TOP (100) PERCENT NEWID() AS ID, dbo.Organisation.ID AS OrganisationID, dbo.Organisation.Name AS Organisation, 
                      dbo.Organisation.Description AS OrganisationDesc, dbo.ProjectSite.ID AS ProjectSiteID, dbo.ProjectSite.Name AS ProjectSite, 
                      dbo.ProjectSite.Description AS ProjectSiteDesc, dbo.Station.ID AS StationID, dbo.Station.Name AS Station, dbo.Station.Description AS StationDesc, 
                      dbo.SensorProcedure.ID AS SensorProcedureID, dbo.SensorProcedure.Name AS SensorProcedure, dbo.SensorProcedure.Description AS SensorProcedureDesc, 
                      dbo.Phenomenon.ID AS PhenomenonID, dbo.Phenomenon.Name AS Phenomenon, dbo.Phenomenon.Description AS PhenomenonDesc, dbo.Offering.ID AS OfferingID, 
                      dbo.Offering.Name AS Offering, dbo.Offering.Description AS OfferingDesc
FROM         dbo.Station INNER JOIN
                      dbo.SensorProcedure ON dbo.SensorProcedure.StationID = dbo.Station.ID INNER JOIN
                      dbo.Phenomenon ON dbo.Phenomenon.ID = dbo.SensorProcedure.PhenomenonID INNER JOIN
                      dbo.PhenomenonOffering ON dbo.PhenomenonOffering.PhenomenonID = dbo.Phenomenon.ID INNER JOIN
                      dbo.Offering ON dbo.Offering.ID = dbo.PhenomenonOffering.OfferingID INNER JOIN
                      dbo.ProjectSite ON dbo.ProjectSite.ID = dbo.Station.ProjectSiteID INNER JOIN
                      dbo.Organisation ON dbo.Organisation.ID = dbo.ProjectSite.OrganisationID
ORDER BY Organisation, ProjectSite, Station, SensorProcedure, Phenomenon, Offering


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Station"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SensorProcedure"
            Begin Extent = 
               Top = 6
               Left = 236
               Bottom = 125
               Right = 396
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Phenomenon"
            Begin Extent = 
               Top = 6
               Left = 654
               Bottom = 125
               Right = 814
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PhenomenonOffering"
            Begin Extent = 
               Top = 6
               Left = 852
               Bottom = 110
               Right = 1014
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Offering"
            Begin Extent = 
               Top = 114
               Left = 434
               Bottom = 233
               Right = 594
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ProjectSite"
            Begin Extent = 
               Top = 114
               Left = 852
               Bottom = 233
               Right = 1013
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Organisation"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 245
               Right = 198
            End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vDataQuery';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 19
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vDataQuery';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vDataQuery';

