import { ApolloServer, gql } from "apollo-server-azure-functions";
import axios from "axios";

// Construct a schema, using GraphQL schema language
const typeDefs = gql`
  type Query {
    hello: String
    allTodos: [Todo]
  }

  type Mutation {
    addTodo(id: ID!, title: String!, completed: Boolean): Todo
    removeTodo(id: ID!): Boolean
    toggleTodo(id: ID!, completed: Boolean!): Todo
  }

  type Todo {
    id: ID
    title: String
    completed: Boolean
  }
`;

// Provide resolver functions for your schema fields
const resolvers = {
  Query: {
    hello: () => "Hello world!",
    allTodos,
  },
  Mutation: {
    addTodo,
    removeTodo,
    toggleTodo,
  },
};

async function allTodos() {
  let todoResponse = await axios.get(`http://localhost:7077/api/todos`);
  return todoResponse.data;
}

async function addTodo(
  _parent: any,
  newTodo: { id: string; title: string; completed: boolean }
) {
  await axios.post(`http://localhost:7077/api/todos`, JSON.stringify(newTodo));
  return newTodo;
}

async function removeTodo(_parent: any, toRemove: { id: string }) {
  let res = await axios.delete(
    `http://localhost:7077/api/todos/${toRemove.id}`
  );
  return res.status === 200;
}

async function toggleTodo(
  _parent: any,
  args: { id: string; completed: boolean }
) {
  await axios.patch(`http://localhost:7077/api/todos/${args.id}/completion`, {
    completed: args.completed,
  });
}

const server = new ApolloServer({ typeDefs, resolvers, playground: false });

exports.graphqlHandler = server.createHandler({
  cors: {
    origin: true,
    credentials: true,
  },
});
