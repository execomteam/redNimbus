import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

class UploadFile extends React.Component {
    constructor(props) {
        super(props);
        this.handleFile = this.handleFile.bind(this);
    }
    state = {
        file: null
    }

    handleFile(e) {

        e.preventDefault();

        let file = e.target.files[0];

    }

    handleUpload(e) {
        let file = this.state.file;

        var reader = new FileReader();
        reader.fileName = this.state.file.name;

        reader.onload = function (readerEvt) {
            console.log(readerEvt.target.fileName);
        };

        const formData = { file: e.target.result };

        reader.readAsBinaryString(file);

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };
        let self = this;
        axios.post("http://localhost:65001/api/bucket/uploadFile", { Value: this.state.file.name, Path: self.props.path, data: formData }, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp) {
        this.props.uploadFile(resp.data);
        this.props.onHide();
    }

    render() {
        return (
            <div>
                <Button variant="primary" onClick={() => this.props.onHide(true)}>
                    Upload File
                </Button>
                <Modal
                    show={this.props.show}
                    size="lg"
                    aria-labelledby="contained-modal-title-vcenter"
                    centered
                >
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            Upload File
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <form id="upload">
                            <div>
                                <label>Select File</label>
                                <input type="file" name="file" onChange={(e) => this.handleFile(e)} />
                            </div>

                        <br />
                            <button type="button" onClick={(e) => this.handleUpload(e)}>Upload</button>
                            <Button onClick={() => this.props.onHide(false)}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>

        )
    }

}

export default UploadFile;