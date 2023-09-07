using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XmlRecursiveObjectParsing;

namespace TestTreeView
{
    public class TestObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public UIControlNode UIControlNode { get; set; }
    }


    public partial class Form1 : Form
    {

        TestObject testObject = new TestObject
        {
            Name = "John",
            Age = 30,
            Attributes = new Dictionary<string, string>
    {
        { "Attribute1", "Value1" },
        { "Attribute2", "Value2" }
    },
            UIControlNode = new UIControlNode
            {
                Id = "mainContainer",
                Type = "container",
                Text = "Container Text",
                Placeholder = "Placeholder Text",
                Src = "image.png",
                Alt = "Image Alt Text",
                Children = new List<UIControlNode>
        {
            new UIControlNode
            {
                Id = "button1",
                Type = "button",
                Text = "Button 1"
            },
            new UIControlNode
            {
                Id = "button2",
                Type = "button",
                Text = "Button 2"
            }
        }
            }
        };

     

        public Form1()
        {
            InitializeComponent();
        }

        UIControlNode selectedNode;
        private void Form1_Load(object sender, EventArgs e)
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
                        </ui>";

            UIControlNode uiModel = XmlParserProgram.ParseXml(xmlString);

            PopulateTreeView(treeView1, uiModel);
        }
        private void PopulateTreeView(TreeView treeView, UIControlNode controlNode)
        {
            TreeNode rootNode = new TreeNode("[ROOT]");
            rootNode.Tag = controlNode;
            treeView.Nodes.Add(rootNode);

            foreach (var childNode in controlNode.Children)
            {
                AddNode(rootNode, childNode);
            }
        }

        private void AddNode(TreeNode parentNode, UIControlNode controlNode)
        {
            TreeNode newNode = new TreeNode(controlNode.Id);
            newNode.Tag = controlNode;
            parentNode.Nodes.Add(newNode);

            foreach (var childNode in controlNode.Children)
            {
                AddNode(newNode, childNode);
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Получаем выбранный узел из TreeView.
            selectedNode = e.Node.Tag as UIControlNode;

            // Обновляем PropertyGrid, чтобы отобразить свойства выбранного узла.
             propertyGrid1.SelectedObject = selectedNode;
           // propertyGrid1.SelectedObject = testObject;
        }
    }
}
