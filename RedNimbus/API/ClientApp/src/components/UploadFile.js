import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

class UploadFile extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            file: null
        }
        this.onFormSubmit = this.onFormSubmit.bind(this);
        this.onChange = this.onChange.bind(this);
        this.onSuccessHandler = this.onSuccessHandler.bind(this);
        this.onErrorHandler = this.onErrorHandler.bind(this);
    }


    

    onFormSubmit(e) {
        e.preventDefault();
        if (this.state.file == null) {
            alert("You must choose valid file before upload.");
        } else {
            helper(this.props.path, this.state.file).then(
                data => {
                    axios.post("http://localhost:65001/api/bucket/uploadFile", data.formData, data.options).then(
                        (resp) => this.onSuccessHandler(resp),
                        (resp) => this.onErrorHandler(resp)
                    );
                }
            );
            
        }
        
    }

    
    onChange(e) {
        e.preventDefault();
        let file_size = e.target.files[0].size;
        if (file_size > 350*1024*1024) {
            alert("File size limit is 350MB. Choose another file please.");
            this.setState({ file: null });
        } else {
            this.setState({ file: e.target.files[0] })
        }
        
    }



    onErrorHandler(resp) {
        alert(resp.response.data.message);
    }

    onSuccessHandler(resp) {
        this.props.uploadFile(resp.data);
        this.props.onHide();
    }

    render() {
        return (
            <div>
                <Button onClick={() => this.props.onHide(true)}>
                    Upload File
                </Button>
                <Modal
                    show={this.props.show}
                    size="lg"
                    aria-labelledby="contained-modal-title-vcenter"
                    centered
                >
                    <Modal.Header >
                        <Modal.Title id="contained-modal-title-vcenter">
                            Upload File
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <form id="upload" onSubmit={this.onFormSubmit}>
                            <div>
                                <label>Select File</label>
                                <input type="file" name="file" id="file" onChange={this.onChange} />
                            </div>

                        <br />
                            <Button type="submit">Upload</Button>
                            <Button onClick={() => this.props.onHide(false)}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>

        )
    }

}

async function helper(path, file) {
    
    const options = {
        headers: {
            'token': localStorage.getItem("token")
        }
    };

    let formData = new FormData();
    formData.append("Path", path);
    formData.append("File", file);
    formData.append("Value", file.name);

    return {
        formData: formData,
        options: options
    }

}

export default UploadFile;