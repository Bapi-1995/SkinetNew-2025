let message: string = 'Hello, TypeScript!';
console.log(message);
type todo = {
    id: number;
    title: string;
    completed: boolean;
}
let myTodo:todo[] =[];
function addTodo(title:string):todo{
    const newTodo:todo={
        id: myTodo.length + 1,
        title: title,
        completed: false};
        myTodo.push(newTodo);
        return newTodo;
}
function toggleTodo(id:number):void{
    const todo=myTodo.find(todo=>todo.id==id);
    if(todo){
        todo.completed=!todo.completed;
    }
}
addTodo('Learn TypeScript');
addTodo('Learn TypeScript1');

console.log(myTodo);
toggleTodo(1);