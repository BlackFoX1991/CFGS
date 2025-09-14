# CFGS Built-in Functions

## 📚 Table of Contents

* [len(arg)](#lenarg)
* [fpos(fileStream)](#fposfilestream)
* [toint32(value)](#toint32value)
* [toint64(value)](#toint64value)
* [chr(value)](#chrvalue)
* [str(value)](#strvalue)
* [todbl(value)](#todblvalue)
* [getl()](#getl)
* [getc()](#getc)
* [getk()](#getk)
* [fopen(path, mode, access)](#fopenpath-mode-access)
* [fwrite(fileStream, content)](#fwritefilestream-content)
* [fread(fileStream)](#freadfilestream)
* [fclose(fileStream)](#fclosefilestream)
* [fexist(path)](#fexistpath)


---

To make things easier, I declared enums so we don’t have to remember all the modifier numbers. Once the language is more polished, I plan to release a standard library that can be imported, containing all essential utilities.

```cfgs
# File modes
enum FileMode
{
    CreateNew = 1;   # Create a new file, error if it exists
    Create;          # Create a new file, overwrite if exists
    Open;            # Open existing file
    OpenOrCreate;    # Open or create if not exists
    Truncate;        # Open and truncate to zero length
    Append;          # Open and write at the end
}

# Access permissions
enum FileAccess
{
    Read = 1;        # Read-only
    Write;           # Write-only
    ReadWrite;       # Read and write
}
```

---

## 📝 len(arg)

Returns the length of an object.

* 📄 Lists → number of elements
* 🔤 Strings → number of characters
* 📁 FileStream → file length in bytes

```cfgs
list = [1, 2, 3];
print(len(list));   # Output: 3

text = "Hello";
print(len(text));   # Output: 5

fl = fopen("test.txt", FileMode.OpenOrCreate, FileAccess.Read);
print(len(fl));     # Output: file length
fclose(fl);
```

---

## 📍 fpos(fileStream)

Returns the current position of the FileStream.

```cfgs
fl = fopen("test.txt", FileMode.OpenOrCreate, FileAccess.Read);
print(fpos(fl));  # Output: 0
fclose(fl);
```

---

## 🔢 toint32(value)

Converts a value to a 32-bit integer.

```cfgs
mystr = "123";  # String variable
num = toint32(mystr);
print(num + 2);  # Output: 125
```

## 🔢 toint64(value)

Converts a value to a 64-bit integer.

```cfgs
mystr = "1234567890123";  # String variable
num = toint64(mystr);
print(num + 2);  # Output: 1234567890125
```

## 🔤 chr(value)

Returns the character corresponding to a Unicode value.

```cfgs
print(chr(65));  # Output: A
```

## 🔤 str(value)

Converts a value to a string.

```cfgs
num = 123;
other_num = 200;
print(str(num) + other_num);  # Output: "123200"
```

## 🟦 todbl(value)

Converts a value to a double.

```cfgs
mystr = "3.14";  # String variable
num = todbl(mystr);
print(num + 0.2); # Output: 3.34
```

## ⌨ getl()

Reads a full line from the console.

```cfgs
input = getl();
print("You entered: " + input);
```

## ⌨ getc()

Reads a single character from the console.

```cfgs
c = getc();
print(c);
```

## ⌨ getk()

Reads a key from the console without pressing Enter.

```cfgs
k = getk();
print(k);  # Output: pressed key as string
```

## 📂 fopen(path, mode, access)

Opens a file with the specified mode (`FileMode`) and access (`FileAccess`).

```cfgs
fl = fopen("test.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
```

## 💾 fwrite(fileStream, content)

Writes the string `content` to the FileStream.

```cfgs
fl = fopen("test.txt", FileMode.OpenOrCreate, FileAccess.Write);
fwrite(fl, "Hello World");
fclose(fl);
```

## 📥 fread(fileStream)

Reads a byte from the FileStream and returns it as an integer.

```cfgs
fl = fopen("test.txt", FileMode.OpenOrCreate, FileAccess.Read);
byte = fread(fl);
print(byte);
fclose(fl);
```

## ❌ fclose(fileStream)

Closes the FileStream.

```cfgs
fl = fopen("test.txt", FileMode.OpenOrCreate, FileAccess.Read);
fclose(fl);
```

## ✅ fexist(path)

Checks if a file exists.

```cfgs
print(fexist("test.txt"));  # true or false
```


[Go back to the Documentation](Docs.md)

