# Baobab Ridge Wildlife Rehabilitation Records System

## Project Overview
A version-controlled C# Windows Forms desktop application built in Visual Studio using .NET version 8.0 or newer[cite: 1]. The system records injured and orphaned wild animals in care, automatically calculates their status and housing unit based on veterinary recovery scores, and produces summary reports for the Centre manager[cite: 1].

## Group Members & Student Numbers
- **Member 1:** Nhluvuko Nkuna - 603544
- **Member 2:** Leila du Plessis - 603582
- **Member 3:** Lourens Jonck - 603246
- **Member 4:** Buhle Hopewell Zitha - 604966

## Features
1. **Add Animal:** Provides labelled input boxes and an Add button to capture animal details. Enforces strict validation rules, checks for unique IDs in the format `WR-` followed by 4 digits, appends new records to `animals.txt`, and displays the calculated status and housing unit[cite: 2, 3].
2. **View Animals:** Automatically loads `animals.txt` on startup and displays valid records in a read-only `DataGridView` using the required column order and headers[cite: 3].
3. **Update Animal:** Loads an existing record for editing either via grid row selection or an exact, case-insensitive ID search. Applies validation, recalculates classification data, and updates only the edited record in `animals.txt`[cite: 4].
4. **Delete Animal:** Safely removes a selected record from the file and grid following a Yes/No confirmation prompt displaying the animal's ID and name[cite: 4].
5. **Generate Summary Report:** Calculates total animals, average age, average recovery score, and counts for all five statuses, displaying them on the form and exporting them to `summary.txt`[cite: 4].

## File Formats
- **`animals.txt`**: A plain text UTF-8 file stored in the same folder as the running `.exe`. It contains one animal per line with seven fields separated by pipe characters (`|`) in this order: `ID | Name | Species | Age | Score | Status | Housing Unit`[cite: 2]. It includes a starter file with at least 8 valid records covering at least 4 different statuses[cite: 2].
- **`summary.txt`**: A text report created or overwritten each time a summary is generated. It follows a strict layout containing a timestamp (`yyyy-MM-dd HH:mm`), total animals, average age, average recovery score, and breakdown counts for all five statuses[cite: 4].

## How to Build and Run
1. Clone the GitHub repository into a new, empty local folder[cite: 5].
2. Open the project solution (`.sln`) file in Visual Studio targeting .NET 8.0 or newer[cite: 1, 5].
3. Ensure the starter `animals.txt` file is included in the project with its property set to **"Copy to Output Directory: Copy if newer"** so it places correctly beside the compiled `.exe`[cite: 2].
4. Build the solution and run the application[cite: 5].

## Known Limitations
- Data storage is restricted exclusively to local plain text files per project constraints; database management systems and storage-related NuGet packages are not allowed[cite: 1].
- Simultaneous multi-user file editing and concurrency control are not supported.
