CREATE VIEW dbo.progress_Progress_Resolved
AS
SELECT     dbo.ImportBatch.ImportDate, dbo.ImportBatch.FileName, dbo.Progress.StartDate, dbo.Progress.EndDate, dbo.Progress.Observations, dbo.Progress.DateUploaded, 
                      dbo.aspnet_Users.UserName, dbo.SensorProcedure.Name AS SensorProcedure, dbo.SensorProcedure.StationID, dbo.Station.Name AS Station
FROM         dbo.SensorProcedure FULL OUTER JOIN
                      dbo.Station ON dbo.SensorProcedure.StationID = dbo.Station.ID FULL OUTER JOIN
                      dbo.Progress LEFT OUTER JOIN
                      dbo.aspnet_Users ON dbo.Progress.UserId = dbo.aspnet_Users.UserId LEFT OUTER JOIN
                      dbo.ImportBatch ON dbo.Progress.ImportBatchID = dbo.ImportBatch.ID ON dbo.SensorProcedure.ID = dbo.Progress.SensorProcedureID

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[49] 4[27] 2[6] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1[56] 3) )"
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
      ActivePaneConfig = 2
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "SensorProcedure"
            Begin Extent = 
               Top = 198
               Left = 241
               Bottom = 379
               Right = 394
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Station"
            Begin Extent = 
               Top = 223
               Left = 488
               Bottom = 331
               Right = 639
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Progress"
            Begin Extent = 
               Top = 15
               Left = 25
               Bottom = 193
               Right = 224
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "aspnet_Users"
            Begin Extent = 
               Top = 23
               Left = 474
               Bottom = 131
               Right = 644
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ImportBatch"
            Begin Extent = 
               Top = 203
               Left = 24
               Bottom = 351
               Right = 175
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 11
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
      End
   End
   Begin CriteriaPane = 
      PaneHidden = ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'progress_Progress_Resolved';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'progress_Progress_Resolved';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'progress_Progress_Resolved';

