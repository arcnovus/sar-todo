export default {
  modules: ["@nuxtjs/apollo"],
  build: {
    // change to CDN with latest contents of /.nuxt/dist/client
    publicPath: "http://localhost:3333",
  },
  apollo: {
    clientConfigs: {
      default: {
        httpEndpoint: "http://localhost:8080/api/graphql",
      },
    },
  },
  css: ["@/assets/styles.css"],
};
