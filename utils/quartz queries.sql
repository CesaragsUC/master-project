-- when you create a Quartz job, some useful informations are saved in this below tables.

SELECT TOP (1000) [SCHED_NAME]
      ,[JOB_NAME]
      ,[JOB_GROUP]
      ,[DESCRIPTION]
      ,[JOB_CLASS_NAME]
      ,[IS_DURABLE]
      ,[IS_NONCONCURRENT]
      ,[IS_UPDATE_DATA]
      ,[REQUESTS_RECOVERY]
      ,[JOB_DATA]
  FROM [quartznet].[dbo].[QRTZ_JOB_DETAILS]

  SELECT * FROM QRTZ_TRIGGERS;

  SELECT * FROM QRTZ_FIRED_TRIGGERS;
 
  SELECT * FROM QRTZ_LOCKS;

  SELECT * FROM QRTZ_SCHEDULER_STATE;

  SELECT * FROM QRTZ_SIMPLE_TRIGGERS;
