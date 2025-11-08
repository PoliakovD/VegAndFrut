namespace VegAndFrut.Models;

// ■ Название;
// ■ Тип (овощ или фрукт);
// ■ Цвет;
// ■ Калорийность.

public record Product(int Id, string Name, string Type, string Color, decimal Calories);