<script>
import Layout from "@/pages/layoutThi/layout";

/**
 * Crypto ICO-landing page
 */
export default {
  components: { Layout },
  data() {
    return {
      data: [],
      apiView: `${process.env.VUE_APP_API_URL}files/view/`,
      url: `${process.env.VUE_APP_API_URL}files/view`,
      totalRows: 1,
      numberOfElement: 1,
      perPage: 9,
      currentPage: 1,
      sortBy: "ngayXuatBan",
      sortDesc: true,
      model: null,
      menu: null,
      filterOn: [],
      isArray: true,
      pageOptions: [5, 10, 25, 50, 100],
      menuName: " ",
      ratings: [1, 2, 3, 4, 5],
      selectedRating: 4, // Giả sử bạn muốn hiển thị 4 sao ban đầu
    };
  },
  computed: {},
  watch: {
    perPage: {
      deep: true,
      handler(val) {
        this.getData();
      },
    },
    currentPage: {
      deep: true,
      handler(val) {
        console.log("this.perpage", this.currentPage);
        this.getData();
      },
    },
  },
  created() {
    window.addEventListener("scroll", this.windowScroll);
    this.getData();
  },
  destroyed() {
    window.removeEventListener("scroll", this.windowScroll);
  },
  mounted() {
    this.start = new Date(this.starttime).getTime();
    this.end = new Date(this.endtime).getTime();
    // Update the count down every 1 second
  },
  methods: {
    async getData() {
      //console.log("LOG GIOI THIEU  :  ")
      const params = {
        start: this.currentPage,
        limit: this.perPage,
        sortBy: "thuTu",
        sortDesc: true,
      };
      await this.$store
        .dispatch("baiThiStore/getPagingParams", params)
        .then((res) => {
          this.model = res.data.data;
          console.log("Bai thi: ", res.data);
          this.totalRows = res.data.totalRows;
          this.numberOfElement = res.data.data.length;
        });
    },
    getColorWithExtFile(ext) {
      if (ext == ".docx" || ext == ".doc") return "text-primary";
      else if (ext == ".xlsx" || ext == ".xls") return "text-success";
      else if (ext == ".pdf") return "text-danger";
      else return "text-danger";
    },

    getIconWithExtFile(ext) {
      if (ext == ".docx" || ext == ".doc") return "mdi mdi-microsoft-word";
      else if (ext == ".xlsx" || ext == ".xls")
        return "mdi mdi-microsoft-excel";
      else if (ext == ".pdf") return "mdi mdi-file-pdf-outline";
      else if (ext == ".pptx") return "fas fa-file-powerpoint";
      else return "mdi-file-clock-outline";
    },

    getUrl(item) {
      // console.log("LOG DANH MUC CLICK", item)
      if (item.link.indexOf("/{id}") < 0 && item.level === 0) {
        // console.log("LOG ROUTER IF LAYOUT  : ", item)
        this.idMenu = item.id;
        //  console.log("LOG ITEM : ", item.link)
        this.$router.push(item.link);
      } else if (item.link.indexOf("/{id}") > 0 && item.level === 0) {
        this.idMenu = item.id;
        // console.log("LOG ROUTER IF ELSE  LAYOUT  : ", item.link.replace("{id}",  item.id))
        this.$router.push(item.link.replace("{id}", item.id));
      } else {
        // console.log("LOG ROUTER ELSE LAYOUT  : ", item.link +   item.id)
        this.idMenu = item.id;
        this.$router.push(item.link + "/" + item.id);
      }
    },

    handleShowRegisterModal() {
      this.$store.dispatch(
        "snackBarStore/showRegisterModal",
        !this.$store.state.snackBarStore.registerModal
      );
      console.log("abc");
    },
    onFiltered(filteredItems) {
      // Trigger pagination to update the number of buttons/pages due to filtering
      this.totalRows = filteredItems.length;
      this.currentPage = 1;
    },
    /**
     * Window scroll method
     */
    windowScroll() {
      const navbar = document.getElementById("navbar");
      if (
        document.body.scrollTop >= 50 ||
        document.documentElement.scrollTop >= 50
      ) {
        navbar.classList.add("nav-sticky");
      } else {
        navbar.classList.remove("nav-sticky");
      }
    },
    /**
     * Toggle menu
     */
    toggleMenu() {
      document.getElementById("topnav-menu-content").classList.toggle("show");
    },
    nextSlide() {
      this.$refs.carousel.goToPage(this.$refs.carousel.getNextPage());
    },
    prevSlide() {
      this.$refs.carousel.goToPage(this.$refs.carousel.getPreviousPage());
    },
    normalizer(node) {
      if (node.children == null || node.children == "null") {
        delete node.children;
      }
    },
  },
};
</script>

<template>
  <Layout>
    <section class="section p-2 bg-white" id="about" style="margin-top: 50px">
      <div class="row align-items-center">
        <div class="container" style="padding: 0px 20px">
          <div class="col-lg-12">
            <div class="cs-title-box">
              <div class="cs-title py-2" style="padding-top: 7px !important">
                <a href="">
                  <i class="mdi mdi-star ic-item"></i>
                  <span
                    style="color: black; font-weight: bold; font-size: 16px"
                  >
                    Trang Tin Bài Viết
                  </span>
                </a>
              </div>
            </div>
          </div>
          <div class="wrap-main col-md-12">
            <div class="row">
              <div class="col-lg-12">
                <div class="row">
                  <div
                    class="col-lg-4 col-md-6 col-sm-12 mb-3 card-item"
                    v-for="(item, index) in model"
                    :key="index"
                  >
                    <router-link
                      :to="{
                        path: `/bai-viet/chi-tiet/${item.id}`,
                      }"
                    >
                      <div v-if="!item.hinhAnh" class="card-img">
                        <div style="height: 100%">
                          <img
                            src="@/assets/images/logo.png"
                            alt="Không có ảnh"
                            style="height: 100%"
                          />
                        </div>
                      </div>
                      <div v-else class="card-img">
                        <div style="height: 100%">
                          <img
                            :src="apiView + item.hinhAnh.fileId"
                            class=""
                            style="height: 100%"
                          />
                        </div>
                      </div>
                      <div class="box-text">
                        <div class="box-info">
                          <p
                            class="name-cate"
                            style="
                              padding-top: 15px;
                              font-weight: bold;
                              font-size: 20px;
                            "
                          >
                            {{ item.name }}
                          </p>
                          <p class="decs-cate">
                            {{ item.moTaNgan }}
                          </p>
                          <div
                            style="
                              display: flex;
                              justify-content: space-between;
                              margin-bottom: 10px;
                            "
                          >
                            <b-button
                              pill
                              class="fs-13 btn-detail class-chitiet"
                              size=""
                            >
                              <i class="bx bx-link-external ps-2"></i>
                              <span class="ps-1">Chi tiết</span>
                              <!-- <i class="mdi mdi-arrow-right ps-2"></i> -->
                            </b-button>
                          </div>
                        </div>
                      </div>
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-lg-12">
              <div class="row mb-3">
                <b-col>
                  <div>
                    Hiển thị {{ numberOfElement }} trên tổng số
                    {{ totalRows }} dòng
                  </div>
                </b-col>
                <div class="col">
                  <div
                    class="dataTables_paginate paging_simple_numbers float-end"
                  >
                    <ul class="pagination pagination-rounded mb-0">
                      <!-- pagination -->
                      <b-pagination
                        v-model="currentPage"
                        :total-rows="totalRows"
                        :per-page="perPage"
                      ></b-pagination>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </Layout>
</template>
<style>
.category {
  background-color: #f9f9f9;
}

.cate-title {
  background-color: #05912a;
  color: #fff;
  padding: 10px;
  font-size: 20px;
}
.class-chitiet {
  background-color: blue;
}
.cate-list ul li {
  list-style: none;
  border-bottom: 1px dashed #d0cfcf;
  padding: 10px 0 10px 0;
}

.cate-list ul li a {
  margin-left: 10px;
  font-size: 14px;
  color: #78797c;
}
.btn-yellow {
  background-color: #efc62c;
  border: none;
  border-radius: 0 !important;
  color: #000 !important;
}

.btn-yellow:hover {
  background-color: #ffc800;
  border: none;
}

.w-10 {
  width: 10%;
}
.w-80 {
  width: 80%;
}
.w-90 {
  width: 90%;
}

.block-ellipsis {
  display: block;
  display: -webkit-box;
  max-width: 100%;
  font-size: 14px;
  line-height: 1.4;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}

tr {
  vertical-align: middle !important;
  box-shadow: 0 0 1rem rgb(18 38 63 / 3%) !important;
}

.bg-ico-primary {
  height: 340px;
}
.ribbons {
  /*background: linear-gradient(45deg, #940012, #F6C6C6);*/
  position: absolute;
  padding: 10px;
  font-weight: bold;
  color: #fff;
  border-radius: 5px;
  top: -18px;
  left: 20px;
  background-color: #2b569a;
  box-shadow: rgba(255, 255, 255, 0) 0px 4px 6px -1px,
    rgba(255, 255, 255, 0.5) 0px 2px 4px -1px;
}

@media only screen and (max-width: 425px) {
  .create-at {
    text-align: start !important;
    margin-bottom: 5px;
  }
}

section.bg-ico-primary {
  padding-top: 100px;
}

.custom-content {
  display: -webkit-box !important;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-body {
  padding: 10px;
}
.card-title {
  font-size: 16px;
  color: #000;
}

.card-img {
  height: 300px;
  padding: 10px 10px 0 10px;
  border-top-right-radius: 10px;
  border-top-left-radius: 10px;
}

.card-img div img {
  border-top-right-radius: 10px;
  border-top-left-radius: 10px;
}

.card-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-top-right-radius: 10px;
  border-top-left-radius: 10px;
}

.card-img-decs {
  height: 300px;
  padding: 10px 10px 0 10px;
  border-top-right-radius: 10px;
  border-top-left-radius: 10px;
}

.card-img-decs img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  border-top-right-radius: 10px;
  border-top-left-radius: 10px;
}
</style>
