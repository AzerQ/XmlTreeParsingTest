using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRecursiveObjectParsing
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public class UIControl
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Placeholder { get; set; }
        public string Src { get; set; }
        public string Alt { get; set; }

        public string Path { get; set; }
    }
        public class UIControlNode: UIControl
    {
        public bool HasChilds { get=> Children.Count > 0; }
        public List<UIControlNode> Children { get; set; } = new List<UIControlNode>();


        public static List<UIControl> FlattenHierarchy(UIControlNode root, string parentPath = "")
        {
            List<UIControl> flatList = new List<UIControl>();
            Stack<Tuple<UIControlNode, string>> stack = new Stack<Tuple<UIControlNode, string>>();
            stack.Push(new Tuple<UIControlNode, string>(root, parentPath));

            while (stack.Count > 0)
            {
                var tuple = stack.Pop();
                UIControlNode currentControl = tuple.Item1;
                string currentPath = tuple.Item2;

                string newPath = currentPath + "/" + currentControl.Id;

                UIControl controlToAdd = new UIControl
                {
                    Id = currentControl.Id,
                    Type = currentControl.Type,
                    Text = currentControl.Text,
                    Placeholder = currentControl.Placeholder,
                    Src = currentControl.Src,
                    Alt = currentControl.Alt,
                    Path = "/" + newPath.Trim('/')
                };

                flatList.Add(controlToAdd);

                foreach (var child in currentControl.Children)
                {
                    stack.Push(new Tuple<UIControlNode, string>(child, newPath));
                }
            }

            return flatList;
        }

        public UIControlNode FindControlByFullPath(string fullPath)
        {
            string[] idArray = fullPath.Trim('/').Split('/');
            UIControlNode currentControl = this;

            foreach (string id in idArray)
            {
                bool found = false;
                foreach (var child in currentControl.Children)
                {
                    if (child.Id == id)
                    {
                        currentControl = child;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return null; // Элемент не найден.
                }
            }

            return currentControl;
        }
    }

   public class XmlParserProgram
    {
        static void Main(string[] args)
        {
            string xmlString = @"<ui>
    <container id='mainContainer'>
        <label id='titleLabel' text='Добро пожаловать' />

        <container id='contentContainer'>
            <button id='button1' text='Кнопка 1' />
            <button id='button2' text='Кнопка 2' />
            <container id='nestedContainer'>
                <input id='textInput' placeholder='Введите текст' />
                <image id='nestedImage' src='nested.jpg' alt='Вложенное изображение' />
            </container>
        </container>

        <container id='sidebarContainer'>
            <button id='sidebarButton' text='Сайдбар' />
            <container id='sidebarMenu'>
                <button id='menuItem1' text='Пункт 1' />
                <button id='menuItem2' text='Пункт 2' />
            </container>
        </container>

        <image id='banner' src='banner.png' alt='Баннер' />
    </container>
</ui>
";


            //string xmlString = Test.XMLFile1;

            UIControlNode uiModel = ParseXml(xmlString);//,out List<UIControl> flatternList);

            // Примеры преобразованных объектов:
            PrintUIModel(uiModel, 0);

            // Пример поиска элемента по полному пути и вывод его свойств:
            string fullPath = "/mainContainer/sidebarContainer/sidebarButton";
            UIControlNode targetControl = uiModel.FindControlByFullPath(fullPath);
            if (targetControl != null)
            {
                Console.WriteLine($"Найден элемент с полным путем '{fullPath}':");
                Console.WriteLine($"Type: {targetControl.Type}");
                Console.WriteLine($"Text: {targetControl.Text}");
                Console.WriteLine($"Placeholder: {targetControl.Placeholder}");
                Console.WriteLine($"Src: {targetControl.Src}");
                Console.WriteLine($"Alt: {targetControl.Alt}");
            }
            else
            {
                Console.WriteLine($"Элемент с полным путем '{fullPath}' не найден.");
            }

            var test1 = UIControlNode.FlattenHierarchy(uiModel);
            foreach (var control in test1.Where(el=>el.Type== "button"))
            {
                Console.WriteLine($"Path: {control.Path}, Id: {control.Id}, Type: {control.Type}, Text: {control.Text}, Placeholder: {control.Placeholder}, Src: {control.Src}, Alt: {control.Alt}");
            }

            Console.ReadLine();
        }

        public static UIControlNode ParseXml(string xmlString )//, out List<UIControl> flatternIerarchy)
        {
            //flatternIerarchy = new List<UIControl>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            XmlNode root = doc.DocumentElement;
            Stack<UIControlNode> stack = new Stack<UIControlNode>();
            Stack<XmlNode> nodeStack = new Stack<XmlNode>();
            UIControlNode uiControl = new UIControlNode(); // Создаем корневой элемент.
            stack.Push(uiControl); // Помещаем корневой элемент в стек.
            nodeStack.Push(root);

            while (nodeStack.Count > 0)
            {
                XmlNode currentNode = nodeStack.Pop();
                UIControlNode currentControl = stack.Pop();

                currentControl.Type = currentNode.Name;
                currentControl.Id = currentNode.Attributes?["id"]?.Value;
                currentControl.Text = currentNode.Attributes?["text"]?.Value;
                currentControl.Placeholder = currentNode.Attributes?["placeholder"]?.Value;
                currentControl.Src = currentNode.Attributes?["src"]?.Value;
                currentControl.Alt = currentNode.Attributes?["alt"]?.Value;

//                flatternIerarchy.Add(currentControl);

                foreach (XmlNode childNode in currentNode.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        UIControlNode childControl = new UIControlNode();
                        currentControl.Children.Add(childControl); // Добавляем потомка к текущему элементу.
                        stack.Push(childControl);
                        nodeStack.Push(childNode);
                    }
                }
            }

            return uiControl; // Возвращаем корневой элемент.
        }

        static void PrintUIModel(UIControlNode control, int level)
        {
            string indentation = new string(' ', level * 4);
            Console.WriteLine($"{indentation}Id: {control.Id}, Type: {control.Type}, Text: {control.Text}, Placeholder: {control.Placeholder}, Src: {control.Src}, Alt: {control.Alt}");

            foreach (var child in control.Children)
            {
                PrintUIModel(child, level + 1);
            }
        }
    }

}
