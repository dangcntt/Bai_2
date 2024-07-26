<script>
import Layout from "@/layouts/main";
import PageHeader from "@/components/page-header";
import { numeric, required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import { pagingModel } from "@/models/pagingModel";
import axios from "axios";

import { baiThiModel } from "@/models/baiThiModel";
export default {
  page: {
    title: "Quản lý bài viết",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: {
    Layout,
    PageHeader,
    "ckeditor-nuxt": () => {
      return import("@blowstack/ckeditor-nuxt");
    },
  },
  data() {
    return {
      title: "Danh sách bài viết",
      items: [
        {
          text: "Tài khoản",
          href: "/tai-khoan",
        },
        {
          text: "Danh sách bài viết",
          active: true,
        },
      ],
      data: [],
      fields: [
        {
          key: "STT",
          label: "STT",
          class: "td-stt",
          sortable: false,
          thClass: "hidden-sortable",
        },
        {
          key: "name",
          label: "Tiêu đề",
          class: "td-title",
          sortable: true,
          thStyle: "text-align:center",
          thClass: "hidden-sortable",
        },
        {
          key: "moTaNgan",
          label: "Mô tả ngắn",
          class: "td-moTaNgan",
          sortable: true,
          thStyle: "text-align:center",
          thClass: "hidden-sortable",
          width: 30,
        },
        // {
        //   key: "noiDung",
        //   label: "Nội dung",
        //   class: 'td-name',
        //   thStyle: "text-align:center",
        //   sortable: true,
        //   thClass: 'hidden-sortable'
        // },
        {
          key: "hinhAnh",
          label: "Hình Ảnh",
          class: "text-center",
          thClass: "hidden-sortable",
        },
        {
          key: "process",
          label: "Xử lý",
          class: "td-xuly",
          sortable: false,
          thClass: "hidden-sortable",
        },
      ],

      showModal: false,
      showModalSlider: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: baiThiModel.baseJson(),
      listRole: [],
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5, 10, 25, 50, 100],
      filter: null,
      todoFilter: null,
      filterOn: [],
      todofilterOn: [],
      sortBy: "age",
      sortDesc: false,
      images: null,
      image: "",
      file: "",
      listFileSilder: [],
      url: `${process.env.VUE_APP_API_URL}files/view/`,

      urlFile: `${process.env.VUE_APP_API_URL}files/view`,
      editorConfig: {
        toolbar: {
          items: [
            "heading",
            "|",
            "style",
            "bold",
            "italic",
            "link",
            "bulletedList",
            "numberedList",
            "|",
            "outdent",
            "indent",
            "|",
            "imageUpload",
            "blockQuote",
            "insertTable",
            "mediaEmbed",
            "codeBlock",
            "alignment",
            "ckbox",
            "fontColor",
            "fontBackgroundColor",
            "fontFamily",
            "fontSize",
            "formatPainter",
            "highlight",
            "htmlEmbed",
            "tableOfContents",
            "undo",
            "redo",
          ],
          shouldNotGroupWhenFull: false,
        },
        removePlugins: ["Title", "ImageCaption"],
        simpleUpload: {
          uploadUrl: process.env.VUE_APP_API_URL + "Files/upload-ck",
          // headers: {
          //   'Authorization': 'optional_token'
          // },
        },
      },
    };
  },
  created() {},
  methods: {
    uploadHinhAnh(file, response) {
      // hinhAnh
      if (event.target && event.target.files.length > 0) {
        const formData = new FormData();
        formData.append("files", event.target.files[0]);
        axios
          .post(`${process.env.VUE_APP_API_URL}files/Upload`, formData)
          .then((response) => {
            // console.log("LOG UPDATE : ", response);
            let resultData = response.data;
            if (response.data.code == 0) {
              this.model.hinhAnh = {
                fileId: resultData.data.fileId,
                fileName: resultData.data.fileName,
                ext: resultData.data.ext,
                path: resultData.data.path,
              };
              console.log("LOG UPDATE : ", this.model);
            }
          })
          .catch((error) => {
            console.error("Error deleting file:", error);
          });
      }
    },

    deleteImage() {
      if (this.model != null && this.model.hinhAnh != null) {
        axios
          .post(
            `${process.env.VUE_APP_API_URL}files/delete/${this.model.hinhAnh.fileId}`
          )
          .then((response) => {
            this.model.hinhAnh = null;
            console.log("log model file remove", this.model.hinhAnh);
          })
          .catch((error) => {
            console.error("Error deleting file:", error);
          });
      }
    },
    getColorWithExtFile(ext) {
      if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
        return "text-danger";
    },

    getIconWithExtFile(ext) {
      if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
        return "mdi mdi-file-image-outline";
    },

    async fnGetList() {
      this.$refs.tblList?.refresh();
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store
          .dispatch("baiThiStore/delete", { id: this.model.id })
          .then((res) => {
            if (res.code === 0) {
              this.fnGetList();
              this.showDeleteModal = false;
            }
            var a = {
              message: res.message,
              code: res.code,
            };
            //   console.log("LOG A : " ,a)
            console.log("Log addNotify handle Delete");
            this.$store.dispatch("snackBarStore/addNotify", {
              message: res.message,
              code: res.code,
            });
          });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },

    async handleSubmitSlider(e) {
      e.preventDefault();
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        console.log("LOG USER UPDATE NE : ", this.model);
        if (
          this.model == null ||
          (this.model != null && this.model.hinhAnh == null)
        ) {
          this.$store.dispatch("snackBarStore/addNotify", {
            message: "Vui lòng chọn file trước khi lưu ",
            code: 20,
          });
          return;
        } else {
          await this.$store
            .dispatch("baiThiStore/update", this.model)
            .then((res) => {
              if (res.code === 0) {
                this.showModalSlider = false;
                this.$refs.tblList.refresh();
                this.model = baiThiModel.baseJson();
              }
              this.$store.dispatch("snackBarStore/addNotify", {
                message: res.message,
                code: res.code,
              });
            });
        }
        // Update model
      } else {
        // Create model
        console.log("LOG USER CREATE NE : ", this.model);
        if (
          this.model == null ||
          (this.model != null && this.model.hinhAnh == null)
        ) {
          this.$store.dispatch("snackBarStore/addNotify", {
            message: "Vui lòng chọn file trước khi lưu ",
            code: 20,
          });
          return;
        } else {
          await this.$store
            .dispatch("baiThiStore/create", this.model)
            .then((res) => {
              if (res.code === 0) {
                this.fnGetList();
                this.showModalSlider = false;
                this.model = baiThiModel.baseJson();
                console.log("log model file CK", this.model);
              }
              this.$store.dispatch("snackBarStore/addNotify", {
                message: res.message,
                code: res.code,
              });
            });
        }
      }

      this.submitted = false;
    },
    async handleUpdate(id) {
      console.log("Log updated id: ", id);
      if (id != 0 && id != null && id) {
        console.log("Log updated id 2: ", id);
        await this.$store
          .dispatch("baiThiStore/getById", { id: id })
          .then((res) => {
            if (res.code === 0) {
              console.log(res);
              this.model = baiThiModel.toJson(res.data);
              this.showModalSlider = true;
            } else {
              this.$store.dispatch("snackBarStore/addNotify", {
                message: res.message,
                code: res.code,
              });
            }
          });
      }
    },

    myProvider(ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortDesc: ctx.sortDesc,
      };
      this.loading = true;
      try {
        console.log("Log content: ", this.filter);
        let promise = this.$store.dispatch(
          "baiThiStore/getPagingParams",
          params
        );

        return promise.then((resp) => {
          console.log("Log res: ", resp);
          let items = resp.data.data;
          this.totalRows = resp.data.totalRows;
          this.numberOfElement = resp.data.data.length;
          this.loading = false;
          return items || [];
        });
      } finally {
        this.loading = false;
      }
    },
  },
  mounted() {
    // this.fnGetList();
    // this.getListDonVi();
  },
  watch: {
    showModalSlider(status) {
      if (status == false) this.model = baiThiModel.baseJson();
    },
    showModal(status) {
      if (status == false) this.model = baiThiModel.baseJson();
    },
    model() {
      return this.model;
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
};
</script>

<template>
  <Layout>
    <PageHeader :title="title" :items="items" />
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <div class="col-sm-4">
                <div class="search-box me-2 mb-2 d-inline-block">
                  <div class="position-relative">
                    <input
                      v-model="filter"
                      type="text"
                      class="form-control"
                      placeholder="Tìm kiếm ..."
                    />
                    <i class="bx bx-search-alt search-icon"></i>
                  </div>
                </div>
              </div>

              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal
                    v-model="showModalSlider"
                    title="Cập nhật bài viết"
                    title-class="text-black font-18"
                    body-class="p-3"
                    hide-footer
                    centered
                    no-close-on-backdrop
                    size="lg"
                  >
                    <form
                      @submit.prevent="handleSubmitSlider"
                      ref="formContainer"
                    >
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-2">
                            <label class="text-left mb-0 text-color"
                              >Tiêu Đề</label
                            >
                            <span style="color: red">&nbsp;*</span>
                            <input
                              v-model="model.name"
                              id="percent"
                              type="text"
                              class="form-control"
                              :class="{
                                'is-invalid': submitted && $v.model.name.$error,
                              }"
                            />
                            <div
                              v-if="submitted && !$v.model.name.required"
                              class="invalid-feedback"
                            >
                              Tiêu đề không được trống.
                            </div>
                          </div>
                        </div>

                        <div class="col-12">
                          <div class="mb-2">
                            <label class="text-left mb-0 text-color"
                              >Mô tả</label
                            >
                            <span style="color: red">&nbsp;*</span>
                            <textarea
                              v-model="model.moTaNgan"
                              id="percent"
                              type="text"
                              class="form-control"
                              :class="{
                                'is-invalid':
                                  submitted && $v.model.moTaNgan.$error,
                              }"
                            >
                            </textarea>
                            <div
                              v-if="submitted && !$v.model.moTaNgan.required"
                              class="invalid-feedback"
                            >
                              Mô tả không được trống.
                            </div>
                          </div>
                        </div>

                        <div class="col-md-12">
                          <div class="mb-2">
                            <label
                              for="formFileSm"
                              class="text-left mb-0 text-color"
                              >Ảnh đại diện bài viết</label
                            >
                            <input
                              id="formFileSm"
                              name="file-input"
                              ref="fileInput"
                              type="file"
                              class="form-control"
                              accept=".jpg,.jpeg,.png,.gif,.bmp"
                              @change="uploadHinhAnh($event)"
                            />
                          </div>
                          <template v-if="model.hinhAnh">
                            <a-
                              class="ml-25"
                              :href="`${urlFile}/${model.hinhAnh.fileId}`"
                              ><i
                                :class="`${getColorWithExtFile(
                                  model.hinhAnh.ext
                                )} me-2 ${getIconWithExtFile(
                                  model.hinhAnh.ext
                                )}`"
                              ></i
                              >{{ model.hinhAnh.fileName }}</a-
                            >
                            <b-button
                              variant="flat-danger"
                              class="btn-icon"
                              @click="deleteImage()"
                            >
                              <i class="mdi mdi-trash-can-outline"></i>
                            </b-button>
                          </template>
                        </div>
                        <div class="col-md-12">
                          <ckeditor-nuxt
                            v-model="model.noiDung"
                            :config="editorConfig"
                          >
                          </ckeditor-nuxt>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button
                          variant="light"
                          @click="showModalSlider = false"
                          class="border-0"
                        >
                          Đóng
                        </b-button>
                        <b-button
                          type="submit"
                          variant="success"
                          class="ms-1 cs-btn-primary"
                          >Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>

              <div style="display: flex; justify-content: space-between">
                <router-link
                  :to="{
                    path: `/danh-sach-trang-tin`,
                  }"
                  >Trang tin bài viết
                </router-link>
                <b-button
                  type="button"
                  class="btn btn-rounded mb-2 me-2"
                  size="sm"
                  @click="showModalSlider = true"
                >
                  <i class="mdi mdi-plus me-1"></i> Thêm bài viết
                </b-button>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <div class="row mt-4">
                  <div class="col-sm-12 col-md-6">
                    <div
                      class="col-sm-12 d-flex justify-content-left align-items-center"
                    >
                      <div
                        id="tickets-table_length"
                        class="dataTables_length m-1"
                        style="
                          display: flex;
                          justify-content: left;
                          align-items: center;
                        "
                      >
                        <div class="me-1">Hiển thị</div>
                        <b-form-select
                          class="form-select form-select-sm"
                          v-model="perPage"
                          size="sm"
                          :options="pageOptions"
                          style="width: 70px"
                        ></b-form-select
                        >&nbsp;
                        <div style="width: 100px">dòng</div>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="table-responsive mb-0">
                  <b-table
                    class="datatables table-admin"
                    :items="myProvider"
                    :fields="fields"
                    striped
                    bordered
                    responsive="sm"
                    :per-page="perPage"
                    :current-page="currentPage"
                    :sort-by.sync="sortBy"
                    :sort-desc.sync="sortDesc"
                    :filter="filter"
                    :filter-included-fields="filterOn"
                    ref="tblList"
                    primary-key="id"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + (currentPage - 1) * perPage + 1 }}
                    </template>

                    <template v-slot:cell(hinhAnh)="data">
                      <div v-if="data.item.hinhAnh != null">
                        <b-card-img
                          :src="url + data.item.hinhAnh.fileId"
                          class="rounded-0 mb-2"
                        ></b-card-img>
                      </div>
                    </template>

                    <template v-slot:cell(process)="data">
                      <button
                        type="button"
                        size="sm"
                        class="btn btn-outline btn-sm"
                        v-on:click="handleUpdate(data.item.id)"
                      >
                        <i class="fas fa-pencil-alt text-success me-1"></i>
                      </button>
                      <button
                        type="button"
                        size="sm"
                        class="btn btn-outline btn-sm"
                        v-on:click="handleShowDeleteModal(data.item.id)"
                      >
                        <i class="fas fa-trash-alt text-danger me-1"></i>
                      </button>
                    </template>
                  </b-table>
                </div>
                <div class="row">
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
            <b-modal
              v-model="showDeleteModal"
              centered
              title="Xóa dữ liệu"
              title-class="font-18"
              no-close-on-backdrop
            >
              <p>Dữ liệu xóa sẽ không được phục hồi!</p>
              <template #modal-footer>
                <button
                  v-b-modal.modal-close_visit
                  class="btn btn-outline-info m-1"
                  v-on:click="showDeleteModal = false"
                >
                  Đóng
                </button>
                <button
                  v-b-modal.modal-close_visit
                  class="btn btn-danger btn m-1"
                  v-on:click="handleDelete"
                >
                  Xóa
                </button>
              </template>
            </b-modal>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style>
.td-xuly {
  text-align: center;
  width: 20%;
}
.btn-rounded {
  margin-top: 10px;
  background-color: blueviolet;
}
</style>
