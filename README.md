# PIS documentation generator

Documentation skeleton generator tool for students of Principles of Information Systems course on FIIT STU.

- Author: Martin Kalužník
- License: MIT

## Usage

1. Export project as .twx file.
2. Pass path to exported file as command line argument to this tool (or simply drag and drop it on compiled binary).
3. Generated text can be found in folder with original .twx file with .txt extension.

## Example of output

```
Get loans data - systémová úloha
    Vstup:
        reminderType - unknown
        teamToken - TeamToken
    Výstup:
        loansData - LoanData
 
    Webová služba: Get loan records after return date
        Názov služby: LibraryServices
        Metóda služby: getBookLoanRecordsAfterReturnDate
        Vstupy:
            teamToken - TeamToken
            date - string
        Výstupy:
            loanRecordsAfterReturnDate - ArrayOfLoanRecordsAfterReturnDate
    Skript: Increase book index
    Skript: Copy loan records
    Rozhodovací blok: Has all loan record data?
```

## Known issues

* User tasks are generated and labeled as system tasks, filter them manually.
* Individual services/scripts/decisions within specific task are generated in arbitrary order, sort them manually as necessary.
* Type of system task input/output variables for builtin types (string, int, ...) is "unknown".
