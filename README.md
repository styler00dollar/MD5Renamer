# MD5Renamer    
<br />

Simple windows console application which scans all files in the directory it is started from and renames them to their own MD5 sum.
If such a filename already exists, the renaming will be skipped.

This fork simply renames files to lowercase MD5 instead of uppercase MD5.

<br />

```
Md5RenamerConsole.exe [.] [(--dry-run|-dr)]

Version 1.0
- without any arguments, this help is displayed.
- the dot is mandatory to prevent accidentally running of the program.
- '--dry-run' or '-d' just writes logs, but doesn't touch a file (case-insensitive).
```
<br />

Example usage:
```
Md5RenamerConsole.exe .
```

Credits to sagerobert/sagerio.
