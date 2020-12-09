@echo off
FOR /F "skip=1 tokens=1-6" %%A IN ('WMIC Path Win32_LocalTime Get Day^,Hour^,Minute^,Month^,Second^,Year /Format:table') DO (
    if "%%B" NEQ "" (
        SET /A FDATE=%%F*10000+%%D*100+%%A
    )
)
SET assver=%1
SET state=%2
SET outp=%3
@echo on
@echo v%assver:~0,3%-%state:~0,3%-%FDATE:~4,4%>"%3version.txt"