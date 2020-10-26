<template>
  <section class="todoapp">
    <h1>Sar Todo</h1>
    <header class="header">
      <input
        class="new-todo"
        autofocus
        type="text"
        autocomplete="off"
        placeholder="What needs to be done?"
        @keyup.enter="addTodo"
        v-model="todoTxt"
      />
    </header>
    <section class="main">
      <ul class="todo-list">
        <li
          v-for="(todo, index) in allTodos"
          :key="todo.id"
          class="todo"
          :class="{ completed: todo.completed }"
        >
          <div class="view">
            <input
              :id="todo.id"
              class="toggle"
              type="checkbox"
              v-model="todo.completed"
              @change="toggleTodo(todo)"
            />
            <label :for="todo.id">{{ todo.title }}</label>
            <button class="destroy" @click="removeTodo(todo, index)"></button>
          </div>
        </li>
      </ul>
    </section>
  </section>
</template>

<script>
import { nanoid } from "nanoid";
import allTodosGQL from "~/apollo/allTodos";
import addTodoGQL from "~/apollo/addTodo";
import removeTodoGQL from "~/apollo/removeTodo";
import toggleTodoGQL from "~/apollo/toggleTodo";

export default {
  async asyncData(context) {
    let client = context.app.apolloProvider.defaultClient;
    let allTodos = (await client.query({ query: allTodosGQL })).data.allTodos;

    return {
      allTodos,
      todoTxt: "",
    };
  },
  methods: {
    addTodo() {
      console.log("attempting addTodo");
      if (!this.todoTxt?.length) return;

      let newTodo = {
        id: nanoid(),
        title: this.todoTxt,
        completed: false,
      };

      // optimistically update the UI
      this.allTodos = [newTodo, ...this.allTodos];
      this.todoTxt = "";

      // asynchronously update the server
      // and back out our UI updates in case of error.
      this.$apollo
        .mutate({
          mutation: addTodoGQL,
          variables: newTodo,
        })
        .catch((err) => {
          this.todoTxt = newTodo.title;
          this.allTodos = this.allTodos.filter(
            (todo) => todo.id !== newTodo.id
          );
        });
    },
    removeTodo(toRemove, index) {
      console.log("attempting removeTodo");
      if (toRemove == null) return;
      // update the UI
      this.allTodos = this.allTodos.filter((todo) => todo.id !== toRemove.id);

      // asynchronously update the server
      // and back out our UI updates in case of error.
      this.$apollo
        .mutate({
          mutation: removeTodoGQL,
          variables: { id: toRemove.id },
        })
        .catch((err) => {
          console.log(err);
          this.allTodos = [toRemove, ...this.allTodos]; // TODO: Preserve order.
        });
    },
    toggleTodo(toToggle) {
      console.log("attempting toggleTodo");

      // UI was updated via two way binding (v-model="todo.completed")
      // so we now asynchronously update the server to match the UI
      // and back out our UI updates in case of error.
      this.$apollo
        .mutate({
          mutation: toggleTodoGQL,
          variables: { id: toToggle.id, completed: toToggle.completed },
        })
        .catch((err) => {
          console.log(err);
          toToggle.completed = !toToggle.completed;
        });
    },
  },
};
</script>
