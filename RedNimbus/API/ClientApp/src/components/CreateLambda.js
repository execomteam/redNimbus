import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';
import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'

const LambdaCreatedAlert = withReactContent(Swal)

class CreateLambda extends React.Component {

    constructor(props) {
        super(props);

        this.createLambda = this.createLambda.bind(this);
        this.onChange = this.onChange.bind(this);
        this.onClose = this.onClose.bind(this);

        this.state = {
            file: null,
            lambdaName: '',
            runtime: '',
            trigger: '',
            isFileSelected: false
        }
    }

    createLambda(event) {

        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        var lambdaName = document.getElementById("lambdaName").value;
        var runtime = document.getElementById("runtime").value;
        var trigger = document.getElementById("trigger").value;

        var formData = new FormData();

        formData.append('file', this.state.file);
        formData.append('Name', lambdaName);
        formData.append('Runtime', runtime);
        formData.append('Trigger', trigger);

        axios.post("http://localhost:65001/api/lambda/create", formData , options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }


    onErrorHandler(resp) {
        LambdaCreatedAlert.fire({
            title: "Error occured",
            text: resp.response.data.message,
            type: "error",
            button: true
          });

    }

    onSuccessHandler(resp) {

        LambdaCreatedAlert.fire({
            title: "Lambda created successfully",
            text: "URL: http://localhost:65001/api/lambda/" + resp.data.guid,
            type: "success",
          });

        this.setState({ redirect: this.state.redirect === false });
        this.props.onHide();
    }

    onChange(e) {
        e.preventDefault();
        this.setState({ file: e.target.files[0], isFileSelected: true })
    }

    onClose(e) {
        e.preventDefault();
        this.setState({ file: null, isFileSelected: false })
        this.props.onHide(false)
    }

    render() {
        return (
            <div>
                <Button variant="primary" onClick={() => this.props.onHide(true)}>
                    Create Lambda
                </Button>
                <Modal
                    show={this.props.show}
                    size="lg"
                    aria-labelledby="contained-modal-title-vcenter"
                    centered = "true"
                >
                    <Modal.Header>
                        <Modal.Title id="contained-modal-title-vcenter">
                            New Lambda
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <form onSubmit={this.createLambda} id="lambdaForm">
                            <div className="form-group">
                                <label htmlFor="lambdaName">Lambda Name</label>
                                <input type="text"
                                    className="form-control form-control-sm"
                                    id="lambdaName"
                                    name="newLambda"
                                    placeholder="Enter name"
                                    required
                                />
                                <br/>
                                <label htmlFor="runtime">Runtime</label>
                                <select id="runtime" name="runtime" className="form-control form-control-sm" required>
                                    <option value="CSHARP">.NET Core 2.1</option>
                                    <option value="PYTHON">Python 3</option>
                                </select>
                                <br />
                                <label htmlFor="trigger">Trigger</label>
                                <select id="trigger" name="trigger" className="form-control form-control-sm" required>
                                    <option value="GET">GET</option>
                                </select>
                                <br />
                                <div>
                                    <label>Select File</label>
                                    <input type="file" name="file" id="file" accept=".zip" onChange={this.onChange} />
                                </div>
                            </div>
                            <hr/>
                            <Button type="submit" disabled={!this.state.isFileSelected}>Create</Button>
                            <Button onClick={this.onClose}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>
        );
    }

}

export default CreateLambda;