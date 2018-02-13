/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) d.[SubjectTag]
      ,s.ExtendedID
	  ,d.[RaceCohort]
      ,d.[CrossSectionalCohort]
      ,d.[Mob]
      ,d.[LastSurveyYearCompleted]
      ,d.[AgeAtLastSurvey]
      ,d.[SimilarAgeCount]
      ,d.[HasMzPossibly]
      ,d.[SiblingPotentialCountInNls]
      ,d.[BirthOrderInNls]
	  ,s.Gender
      --,d.[KidCountBio]
      --,d.[IsDead]
      --,d.[DeathDate]
      --,d.[IsBiodadDead]
      --,d.[BiodadDeathDate]

	  --  delete

  FROM [NlsyLinks97].[Process].[tblSubjectDetails] d
  left join Process.tblSubject s on d.SubjectTag=s.SubjectTag
  where HasMzPossibly=1
  order by ExtendedID, s.SubjectTag
