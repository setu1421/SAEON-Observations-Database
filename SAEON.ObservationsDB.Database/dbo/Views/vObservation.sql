CREATE VIEW [dbo].[vObservation]
AS
SELECT     o.ID, o.SensorProcedureID, o.PhenonmenonOfferingID, o.PhenonmenonUOMID, o.UserId, o.RawValue, o.DataValue, o.ImportBatchID, o.ValueDate, 
                      sp.Code AS spCode, sp.Description AS spDesc, sp.Name AS spName, sp.Url AS spURL, sp.DataSchemaID, sp.DataSourceID, sp.PhenomenonID, sp.StationID, 
                      ph.Name AS phName, st.Name AS stName, ds.Name AS dsName, ISNULL(dschema.Name,dschema1.Name) AS dschemaName, offer.Name AS offName, offer.ID AS offID, ps.Name AS psName, 
                      ps.ID AS psID, org.Name AS orgName, org.ID AS orgID, uom.Unit AS uomUnit, uom.UnitSymbol AS uomSymbol, users.UserName,
                      o.Comment
FROM         dbo.Observation AS o INNER JOIN
                      dbo.SensorProcedure AS sp ON sp.ID = o.SensorProcedureID INNER JOIN
                      dbo.Phenomenon AS ph ON ph.ID = sp.PhenomenonID INNER JOIN
                      dbo.PhenomenonOffering AS phOff ON phOff.ID = o.PhenonmenonOfferingID INNER JOIN  
                      dbo.Offering AS offer ON offer.ID = phOff.OfferingID INNER JOIN
                      dbo.PhenomenonUOM AS puom ON puom.ID = o.PhenonmenonUOMID INNER JOIN
                      dbo.Station AS st ON st.ID = sp.StationID INNER JOIN
                      dbo.DataSource AS ds ON ds.ID = sp.DataSourceID LEFT JOIN
                      dbo.DataSchema AS dschema1 ON dschema1.ID = ds.DataSchemaID LEFT JOIN
                      dbo.DataSchema AS dschema ON dschema.ID = sp.DataSchemaID INNER JOIN                   
                      dbo.ProjectSite AS ps ON ps.ID = st.ProjectSiteID INNER JOIN
                      dbo.Organisation AS org ON org.ID = ps.OrganisationID INNER JOIN
                      dbo.UnitOfMeasure AS uom ON uom.ID = puom.UnitOfMeasureID INNER JOIN
                      dbo.aspnet_Users AS users ON users.UserId = o.UserId 
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
         Begin Table = "o"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 246
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sp"
            Begin Extent = 
               Top = 6
               Left = 284
               Bottom = 125
               Right = 446
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ph"
            Begin Extent = 
               Top = 6
               Left = 484
               Bottom = 125
               Right = 644
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "st"
            Begin Extent = 
               Top = 6
               Left = 682
               Bottom = 125
               Right = 842
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ds"
            Begin Extent = 
               Top = 6
               Left = 880
               Bottom = 125
               Right = 1047
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "dschema"
            Begin Extent = 
               Top = 6
               Left = 1085
               Bottom = 125
               Right = 1265
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "phOff"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 230
               Right = 200
            End
            DisplayFlags = 280
            TopColumn = 0
    ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vObservation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'     End
         Begin Table = "offer"
            Begin Extent = 
               Top = 126
               Left = 238
               Bottom = 245
               Right = 398
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ps"
            Begin Extent = 
               Top = 126
               Left = 436
               Bottom = 245
               Right = 597
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "org"
            Begin Extent = 
               Top = 126
               Left = 635
               Bottom = 245
               Right = 795
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "puom"
            Begin Extent = 
               Top = 126
               Left = 833
               Bottom = 245
               Right = 1005
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "uom"
            Begin Extent = 
               Top = 126
               Left = 1043
               Bottom = 245
               Right = 1203
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "users"
            Begin Extent = 
               Top = 234
               Left = 38
               Bottom = 353
               Right = 217
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 30
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vObservation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vObservation';

