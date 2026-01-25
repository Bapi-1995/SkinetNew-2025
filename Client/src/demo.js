var message = 'Hello, TypeScript!';
console.log(message);
var myTodo = [];
function addTodo(title) {
    var newTodo = {
        id: myTodo.length + 1,
        title: title,
        completed: false
    };
    myTodo.push(newTodo);
    return newTodo;
}
function toggleTodo(id) {
    var todo = myTodo.find(function (todo) { return todo.id == id; });
    if (todo) {
        todo.completed = !todo.completed;
    }
}
addTodo('Learn TypeScript');
addTodo('Learn TypeScript1');
console.log(myTodo);
toggleTodo(1);
