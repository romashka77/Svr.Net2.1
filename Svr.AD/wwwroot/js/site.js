// Первая заглавная, остальные строчные
function textFirstUpperCase(text)
{
  if (text.value.length > 0)
  {
    text.value = text.value.charAt(0).toUpperCase() + text.value.substr(1, text.value.length - 1).toLowerCase();
  }
}