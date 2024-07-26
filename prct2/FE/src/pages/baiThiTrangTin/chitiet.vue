<script>
import Layout from "@/pages/layoutThi/layout";
import Multiselect from "vue-multiselect";
import VueEasyLightbox from "vue-easy-lightbox";
import VueSlickCarousel from "vue-slick-carousel";
import "vue-slick-carousel/dist/vue-slick-carousel.css";
import "vue-slick-carousel/dist/vue-slick-carousel-theme.css";

/**
 * Crypto ICO-landing page
 */
export default {
  components: { Layout },
  data() {
    return {
      data: [],
      start: 2,
      // url: `${process.env.VUE_APP_API_URL}files/view`,
      totalRows: 1,
      numberOfElement: 1,
      perPage: 10,
      currentPage: 1,
      sortBy: "age",
      sortDesc: false,
      model: [],
      filterOn: [],
      pageOptions: [5, 10, 25, 50, 100],
      url: `${process.env.VUE_APP_API_URL}files/view`,
      apiView: `${process.env.VUE_APP_API_URL}files/view/`,
      listPicture: null,
      ImagesShow: [],
      pictures: null,
      indexImage: 0,
      visible: false,
      keyFile: 0,
      index: 0,
      slickOptions: {
        slidesToShow: 3,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 1000,
      },
      settings: {
        arrows: false,
        dots: false,
        infinite: false,
        speed: 500,
        slidesToShow: 6,
        slidesToScroll: 1,
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 6,
              slidesToScroll: 3,
              infinite: true,
              dots: false,
            },
          },
          {
            breakpoint: 600,
            settings: {
              slidesToShow: 5,
              slidesToScroll: 4,
              initialSlide: 4,
              dots: false,
            },
          },
          {
            breakpoint: 480,
            settings: {
              slidesToShow: 5,
              slidesToScroll: 3,
              dots: false,
            },
          },
        ],
      },
    };
  },
  computed: {},
  watch: {
    $route(to, from) {
      this.getData();
    },
    "model._id": function (newVal, oldVal) {
      // Handle the change in _id here
      console.log(`_id changed from ${oldVal} to ${newVal}`);
    },
  },
  created() {
    console.log("LOG CREATED ");
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
      await this.$store
        .dispatch("baiThiStore/getById", {
          id: this.$route.params.id,
        })
        .then((res) => {
          this.model = res.data;
          this.pictures = this.model.hinhAnh;

          console.log("Log mode:", this.model);
          if (this.model.moTaNgan && this.model.moTaNgan != null) {
            this.model.moTaNgan = this.model.moTaNgan.replaceAll(
              "https://localhost:5001/api/v1/files/view",
              this.url
            );
          }
          if (res.data.file && res.data.file != null) {
            // Giả sử res.data.file chỉ chứa một hình ảnh duy nhất.
            const element = res.data.hinhAnh;

            // this.listPicture = element;
            // this.ImagesShow(this.apiView + element?.fileId);
            // console.log('Log url id image: ' + this.apiView + element?.fileId);
          }
        });
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

    HandleClickImage(index) {
      this.indexImage = index;
    },

    showImg(indexImage) {
      this.keyFile = indexImage;
      this.visible = true;
    },

    handleHide() {
      this.visible = false;
    },

    showPreview() {
      const preview = this.$imagePreview({
        initIndex: 0,
        images: this.relink,
        isEnableBlurBackground: false,
        isEnableLoopToggle: true,
        initViewMode: "contain",
        containScale: 1,
        shirnkAndEnlargeDeltaRatio: 0.2,
        wheelScrollDeltaRatio: 0.2,
        isEnableImagePageIndicator: true,
        maskBackgroundColor: "rgba(0,0,0,0.6)",
        zIndex: 4000,
        isEnableDownloadImage: true,
      });
      this.relink = [];
    },
  },
};
</script>

<template>
  <Layout>
    <section class="section p-2 bg-white" id="about" style="margin-top: 50px">
      <div class="row align-items-center">
        <div>
          <div class="row">
            <div class="col-12">
              <div class="row">
                <div class="col-lg-12">
                  <div class="container">
                    <div class="row">
                      <div class="col-lg-12 mb-3">
                        <div class="row mb-3">
                          <div
                            class="col-lg-4 col-md-6 col-sm-12 mb-3 card-item"
                          ></div>

                          <div
                            class="col-md-6 mt-2"
                            style="font-size: 15px; font-weight: bold"
                          >
                            <div class="p-1">
                              <span class="text-black">
                                <strong class="color">
                                  {{ this.model.name }}
                                </strong>
                              </span>
                            </div>
                          </div>
                        </div>

                        <div
                          v-html="model.moTaNgan"
                          class="noidung"
                          style="font-size: 14px"
                        ></div>
                        <div
                          v-html="model.noiDung"
                          class="noidung"
                          style="font-size: 14px"
                        ></div>
                      </div>
                    </div>
                    <div
                      class="row d-flex align-items-baseline mb-3 class-quaylai"
                    >
                      <div class="col-12 position-relative pt-4">
                        <a class="back" @click="$router.go(-1)">
                          <i class="bx bx-left-arrow-alt me-2 text-white"></i>
                          Quay lại
                        </a>
                      </div>
                    </div>
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

.cate-list ul li {
  list-style: none;
  border-bottom: 1px dashed #d0cfcf;
  padding: 10px 0 10px 0;
}
.class-quaylai {
  cursor: pointer;
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

.color-primary {
  /*color: #28883b;*/
  color: #2b569a;
}

.bg-primary {
  /*background-color: #28883b !important;*/
  background-color: #2b569a !important;
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

.btn-detail {
  background: #2b569a;
  border: none;
}

.btn-secondary {
  --bs-btn-bg: #2b569a;
  --bs-btn-hover-bg: #537961;
}

.custom-content {
  display: -webkit-box !important;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.image-tracuu-pakn {
  border-bottom-left-radius: 10px !important;
  border-top-left-radius: 10px !important;
  width: -webkit-fill-available;
}

.detail {
  font-size: 14px;
}

.detail .image img {
  width: 80%;
}

.detail .image {
  text-align: center;
}

.noidung figure {
  overflow: unset;
  aspect-ratio: unset;
  margin: 0 0 1rem;
}
</style>
