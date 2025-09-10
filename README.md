# ConfigScript (CFGS)

![Status](https://img.shields.io/badge/status-in%20development-yellow)
![Language](https://img.shields.io/badge/language-CFGS-blue)
![License](https://img.shields.io/badge/license-Private-lightgrey)

**ConfigScript (CFGS)** is a lightweight custom scripting language designed to make configuring programs simple and flexible.

---

## Features

- **Conditional statements:** `if`, `else if`, `else` (curly braces required for every statement)  
- **Loops:** `while` loops with `break` and `continue` (curly braces required)  
- **Functions** for reusable code  
- **Structs** (basic support)  
- **Importing other scripts** for modularity  
- **Semicolon required** after every statement, except at the end of a block  

---

## Example

```cfgs
# Example ConfigScript code
function greet(name) {
    if (name == "Alice") {
        print("Hello Alice!");
    } else {
        print("Hello stranger!");
    }
}

while (true) {
    greet("Alice");
    break;
}
```

---

## Roadmap

- Add more advanced features  
- Fix existing bugs  
- Improve stability and performance  

---

## Notes

> ⚠️ ConfigScript is currently private and under active development. It will remain private until it is stable enough for public use.

