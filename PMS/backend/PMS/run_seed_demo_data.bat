@echo off
REM Run this file from the same folder as seed_demo_data.sql
REM It fills the PMS_Database database on localhost\SQLEXPRESS with demo data.

sqlcmd -S localhost\SQLEXPRESS -d PMS_Database -E -i "%~dp0seed_demo_data.sql"

pause
